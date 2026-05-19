using System.Globalization;
using BioInfoProject.Server.Csv;
using BioInfoProject.Server.Data;
using BioInfoProject.Server.Entities;
using BioInfoProject.Server.Services.Interfaces;
using BioInfoProject.Shared.DTOs;
using CsvHelper;
using CsvHelper.Configuration;

namespace BioInfoProject.Server.Services;

public class CsvImportService : ICsvImportService
{
    private static readonly string[] RequiredColumns =
    [
        "date_reported",
        "country_code",
        "country",
        "who_region",
        "new_cases",
        "cumulative_cases",
        "new_deaths",
        "cumulative_deaths"
    ];

    private static readonly string[] DateFormats =
    [
        "yyyy-MM-dd",
        "yyyy/MM/dd",
        "MM/dd/yyyy",
        "M/d/yyyy",
        "dd/MM/yyyy",
        "d/M/yyyy"
    ];

    private readonly BioInfoDbContext _dbContext;

    public CsvImportService(BioInfoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ImportResultDto> ImportAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file.Length == 0)
        {
            return ImportResultDtoFailure("The uploaded CSV file is empty.");
        }

        if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return ImportResultDtoFailure("Only CSV files are supported.");
        }

        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CreateCsvConfiguration());

        csv.Context.RegisterClassMap<EpidemiologicalCsvRowMap>();

        if (!await csv.ReadAsync())
        {
            return ImportResultDtoFailure("The uploaded CSV file does not contain a header row.");
        }

        csv.ReadHeader();
        var missingColumns = GetMissingRequiredColumns(csv.HeaderRecord);
        if (missingColumns.Count > 0)
        {
            return ImportResultDtoFailure(
                "The CSV file is missing required columns.",
                missingColumns.Select(column => $"Missing column: {column}"));
        }

        var records = new List<EpidemiologicalRecord>();
        var errors = new List<string>();

        while (await csv.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rowNumber = csv.Context.Parser?.Row ?? 0;
            EpidemiologicalCsvRow row;

            try
            {
                row = csv.GetRecord<EpidemiologicalCsvRow>();
            }
            catch (Exception ex) when (ex is CsvHelperException or InvalidOperationException)
            {
                errors.Add($"Row {rowNumber}: could not read row values.");
                continue;
            }

            if (TryCreateRecord(row, rowNumber, errors, out var record))
            {
                records.Add(record);
            }
        }

        if (records.Count == 0)
        {
            return ImportResultDtoFailure(
                "No valid rows were found in the CSV file.",
                errors);
        }

        var storedFileName = $"{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}";
        var dataset = CreateDataset(file.FileName, storedFileName, records);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        _dbContext.Datasets.Add(dataset);
        await _dbContext.SaveChangesAsync(cancellationToken);

        foreach (var record in records)
        {
            record.DatasetId = dataset.Id;
        }

        _dbContext.EpidemiologicalRecords.AddRange(records);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new ImportResultDto
        {
            Success = true,
            Message = "CSV file imported successfully.",
            DatasetId = dataset.Id,
            FileName = dataset.OriginalFileName,
            ImportedRows = records.Count,
            SkippedRows = errors.Count,
            Errors = errors
        };
    }

    private static CsvConfiguration CreateCsvConfiguration()
    {
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true,
            PrepareHeaderForMatch = args => args.Header.Trim().ToLowerInvariant(),
            HeaderValidated = null,
            MissingFieldFound = null,
            BadDataFound = null,
            TrimOptions = TrimOptions.Trim
        };
    }

    private static List<string> GetMissingRequiredColumns(string[]? headers)
    {
        var normalizedHeaders = (headers ?? Array.Empty<string>())
            .Select(header => header.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return RequiredColumns
            .Where(column => !normalizedHeaders.Contains(column))
            .ToList();
    }

    private static bool TryCreateRecord(
        EpidemiologicalCsvRow row,
        int rowNumber,
        List<string> errors,
        out EpidemiologicalRecord record)
    {
        record = null!;

        var country = CleanText(row.Country);
        var whoRegion = CleanText(row.WhoRegion);
        var countryCode = CleanText(row.CountryCode).ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(country))
        {
            errors.Add($"Row {rowNumber}: country is required.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(whoRegion))
        {
            errors.Add($"Row {rowNumber}: who_region is required.");
            return false;
        }

        if (!TryParseDate(row.DateReported, out var dateReported))
        {
            errors.Add($"Row {rowNumber}: invalid date_reported value.");
            return false;
        }

        if (!TryParseWholeNumber(row.NewCases, out var newCases) ||
            !TryParseWholeNumber(row.CumulativeCases, out var cumulativeCases) ||
            !TryParseWholeNumber(row.NewDeaths, out var newDeaths) ||
            !TryParseWholeNumber(row.CumulativeDeaths, out var cumulativeDeaths))
        {
            errors.Add($"Row {rowNumber}: case and death values must be whole numbers.");
            return false;
        }

        record = new EpidemiologicalRecord
        {
            DateReported = dateReported,
            CountryCode = countryCode,
            Country = country,
            WhoRegion = whoRegion,
            NewCases = newCases,
            CumulativeCases = cumulativeCases,
            NewDeaths = newDeaths,
            CumulativeDeaths = cumulativeDeaths
        };

        return true;
    }

    private static Dataset CreateDataset(string originalFileName, string storedFileName, IReadOnlyCollection<EpidemiologicalRecord> records)
    {
        return new Dataset
        {
            FileName = storedFileName,
            OriginalFileName = Path.GetFileName(originalFileName),
            UploadedAt = DateTime.UtcNow,
            RowCount = records.Count,
            CountryCount = records
                .Select(record => record.Country)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count(),
            RegionCount = records
                .Select(record => record.WhoRegion)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count(),
            MinDate = records.Min(record => record.DateReported),
            MaxDate = records.Max(record => record.DateReported)
        };
    }

    private static string CleanText(string value)
    {
        return value.Trim();
    }

    private static bool TryParseWholeNumber(string value, out int result)
    {
        value = value.Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            result = 0;
            return true;
        }

        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
    }

    private static bool TryParseDate(string value, out DateOnly result)
    {
        value = value.Trim();

        if (DateOnly.TryParseExact(
            value,
            DateFormats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out result))
        {
            return true;
        }

        return DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
    }

    private static ImportResultDto ImportResultDtoFailure(string message, IEnumerable<string>? errors = null)
    {
        return new ImportResultDto
        {
            Success = false,
            Message = message,
            Errors = errors?.ToList() ?? new List<string>()
        };
    }
}
