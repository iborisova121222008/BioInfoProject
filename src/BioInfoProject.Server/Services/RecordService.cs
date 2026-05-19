using BioInfoProject.Server.Data;
using BioInfoProject.Server.Entities;
using BioInfoProject.Server.Services.Interfaces;
using BioInfoProject.Shared.DTOs;
using BioInfoProject.Shared.Requests;
using Microsoft.EntityFrameworkCore;

namespace BioInfoProject.Server.Services;

public class RecordService : IRecordService
{
    private readonly BioInfoDbContext _dbContext;

    public RecordService(BioInfoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResultDto<EpidemiologicalRecordDto>> GetFilteredAsync(
        RecordFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = ApplyFilters(_dbContext.EpidemiologicalRecords.AsNoTracking(), request);
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await ApplySorting(query, request.SortBy, request.SortDirection)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(record => MapToDto(record))
            .ToListAsync(cancellationToken);

        return new PagedResultDto<EpidemiologicalRecordDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IReadOnlyList<string>> GetCountriesAsync(int datasetId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EpidemiologicalRecords
            .AsNoTracking()
            .Where(record => record.DatasetId == datasetId)
            .Select(record => record.Country)
            .Distinct()
            .OrderBy(country => country)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetRegionsAsync(int datasetId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EpidemiologicalRecords
            .AsNoTracking()
            .Where(record => record.DatasetId == datasetId)
            .Select(record => record.WhoRegion)
            .Distinct()
            .OrderBy(region => region)
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<EpidemiologicalRecord> ApplyFilters(
        IQueryable<EpidemiologicalRecord> query,
        RecordFilterRequest request)
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

    private static IQueryable<EpidemiologicalRecord> ApplySorting(
        IQueryable<EpidemiologicalRecord> query,
        string sortBy,
        string sortDirection)
    {
        var descending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLowerInvariant() switch
        {
            "country" => descending
                ? query.OrderByDescending(record => record.Country)
                : query.OrderBy(record => record.Country),
            "whoregion" or "who_region" => descending
                ? query.OrderByDescending(record => record.WhoRegion)
                : query.OrderBy(record => record.WhoRegion),
            "newcases" or "new_cases" => descending
                ? query.OrderByDescending(record => record.NewCases)
                : query.OrderBy(record => record.NewCases),
            "cumulativecases" or "cumulative_cases" => descending
                ? query.OrderByDescending(record => record.CumulativeCases)
                : query.OrderBy(record => record.CumulativeCases),
            "newdeaths" or "new_deaths" => descending
                ? query.OrderByDescending(record => record.NewDeaths)
                : query.OrderBy(record => record.NewDeaths),
            "cumulativedeaths" or "cumulative_deaths" => descending
                ? query.OrderByDescending(record => record.CumulativeDeaths)
                : query.OrderBy(record => record.CumulativeDeaths),
            _ => descending
                ? query.OrderByDescending(record => record.DateReported)
                : query.OrderBy(record => record.DateReported)
        };
    }

    private static EpidemiologicalRecordDto MapToDto(EpidemiologicalRecord record)
    {
        return new EpidemiologicalRecordDto
        {
            Id = record.Id,
            DatasetId = record.DatasetId,
            DateReported = record.DateReported,
            CountryCode = record.CountryCode,
            Country = record.Country,
            WhoRegion = record.WhoRegion,
            NewCases = record.NewCases,
            CumulativeCases = record.CumulativeCases,
            NewDeaths = record.NewDeaths,
            CumulativeDeaths = record.CumulativeDeaths
        };
    }
}
