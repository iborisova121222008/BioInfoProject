using System.Globalization;
using System.Text;
using BioInfoProject.Server.Data;
using BioInfoProject.Server.Entities;
using BioInfoProject.Server.Services.Interfaces;
using BioInfoProject.Shared.Requests;
using CsvHelper;
using Microsoft.EntityFrameworkCore;

namespace BioInfoProject.Server.Services;

public class ExportService : IExportService
{
    private readonly BioInfoDbContext _dbContext;

    public ExportService(BioInfoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<byte[]> ExportRecordsAsync(ExportRequest request, CancellationToken cancellationToken = default)
    {
        var records = await ApplyFilters(_dbContext.EpidemiologicalRecords.AsNoTracking(), request)
            .OrderBy(record => record.DateReported)
            .ThenBy(record => record.Country)
            .ToListAsync(cancellationToken);

        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteField("date_reported");
        csv.WriteField("country_code");
        csv.WriteField("country");
        csv.WriteField("who_region");
        csv.WriteField("new_cases");
        csv.WriteField("cumulative_cases");
        csv.WriteField("new_deaths");
        csv.WriteField("cumulative_deaths");
        await csv.NextRecordAsync();

        foreach (var record in records)
        {
            csv.WriteField(record.DateReported.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            csv.WriteField(record.CountryCode);
            csv.WriteField(record.Country);
            csv.WriteField(record.WhoRegion);
            csv.WriteField(record.NewCases);
            csv.WriteField(record.CumulativeCases);
            csv.WriteField(record.NewDeaths);
            csv.WriteField(record.CumulativeDeaths);
            await csv.NextRecordAsync();
        }

        await writer.FlushAsync(cancellationToken);
        return stream.ToArray();
    }

    private static IQueryable<EpidemiologicalRecord> ApplyFilters(
        IQueryable<EpidemiologicalRecord> query,
        ExportRequest request)
    {
        query = query.Where(record => record.DatasetId == request.DatasetId);

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            query = query.Where(record => record.Country == request.Country);
        }

        if (!string.IsNullOrWhiteSpace(request.WhoRegion))
        {
            query = query.Where(record => record.WhoRegion == request.WhoRegion);
        }

        if (request.StartDate.HasValue)
        {
            query = query.Where(record => record.DateReported >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(record => record.DateReported <= request.EndDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim();
            query = query.Where(record =>
                record.Country.Contains(searchTerm) ||
                record.CountryCode.Contains(searchTerm) ||
                record.WhoRegion.Contains(searchTerm));
        }

        return query;
    }
}
