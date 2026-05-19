using BioInfoProject.Shared.DTOs;
using BioInfoProject.Shared.Requests;

namespace BioInfoProject.Server.Services.Interfaces;

public interface IAnalyticsService
{
    Task<DashboardStatisticsDto> GetDashboardStatisticsAsync(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TimeSeriesPointDto>> GetTimeSeriesAsync(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CountrySummaryDto>> GetCountrySummariesAsync(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RegionSummaryDto>> GetRegionSummariesAsync(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken = default);
}
