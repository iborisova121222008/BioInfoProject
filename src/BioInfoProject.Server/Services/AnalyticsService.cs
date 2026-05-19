using BioInfoProject.Server.Data;
using BioInfoProject.Server.Entities;
using BioInfoProject.Server.Services.Interfaces;
using BioInfoProject.Shared.DTOs;
using BioInfoProject.Shared.Requests;
using Microsoft.EntityFrameworkCore;

namespace BioInfoProject.Server.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly BioInfoDbContext _dbContext;

    public AnalyticsService(BioInfoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilters(_dbContext.EpidemiologicalRecords.AsNoTracking(), request);

        if (!await query.AnyAsync(cancellationToken))
        {
            return new DashboardStatisticsDto();
        }

        return new DashboardStatisticsDto
        {
            TotalNewCases = await query.SumAsync(record => (long)record.NewCases, cancellationToken),
            MaxCumulativeCases = await query.MaxAsync(record => record.CumulativeCases, cancellationToken),
            TotalNewDeaths = await query.SumAsync(record => (long)record.NewDeaths, cancellationToken),
            MaxCumulativeDeaths = await query.MaxAsync(record => record.CumulativeDeaths, cancellationToken),
            CountryCount = await query.Select(record => record.Country).Distinct().CountAsync(cancellationToken),
            RegionCount = await query.Select(record => record.WhoRegion).Distinct().CountAsync(cancellationToken),
            StartDate = await query.MinAsync(record => record.DateReported, cancellationToken),
            EndDate = await query.MaxAsync(record => record.DateReported, cancellationToken)
        };
    }

    public async Task<IReadOnlyList<TimeSeriesPointDto>> GetTimeSeriesAsync(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(_dbContext.EpidemiologicalRecords.AsNoTracking(), request)
            .GroupBy(record => record.DateReported)
            .OrderBy(group => group.Key)
            .Select(group => new TimeSeriesPointDto
            {
                Date = group.Key,
                NewCases = group.Sum(record => (long)record.NewCases),
                CumulativeCases = group.Max(record => record.CumulativeCases),
                NewDeaths = group.Sum(record => (long)record.NewDeaths),
                CumulativeDeaths = group.Max(record => record.CumulativeDeaths)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CountrySummaryDto>> GetCountrySummariesAsync(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(_dbContext.EpidemiologicalRecords.AsNoTracking(), request)
            .GroupBy(record => new { record.Country, record.CountryCode })
            .Select(group => new CountrySummaryDto
            {
                Country = group.Key.Country,
                CountryCode = group.Key.CountryCode,
                TotalNewCases = group.Sum(record => (long)record.NewCases),
                MaxCumulativeCases = group.Max(record => record.CumulativeCases),
                TotalNewDeaths = group.Sum(record => (long)record.NewDeaths),
                MaxCumulativeDeaths = group.Max(record => record.CumulativeDeaths)
            })
            .OrderByDescending(summary => summary.TotalNewCases)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RegionSummaryDto>> GetRegionSummariesAsync(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        return await ApplyFilters(_dbContext.EpidemiologicalRecords.AsNoTracking(), request)
            .GroupBy(record => record.WhoRegion)
            .Select(group => new RegionSummaryDto
            {
                WhoRegion = group.Key,
                CountryCount = group.Select(record => record.Country).Distinct().Count(),
                TotalNewCases = group.Sum(record => (long)record.NewCases),
                MaxCumulativeCases = group.Max(record => record.CumulativeCases),
                TotalNewDeaths = group.Sum(record => (long)record.NewDeaths),
                MaxCumulativeDeaths = group.Max(record => record.CumulativeDeaths)
            })
            .OrderByDescending(summary => summary.TotalNewCases)
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<EpidemiologicalRecord> ApplyFilters(
        IQueryable<EpidemiologicalRecord> query,
        AnalyticsFilterRequest request)
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

        return query;
    }
}
