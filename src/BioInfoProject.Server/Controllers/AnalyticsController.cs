using BioInfoProject.Server.Services.Interfaces;
using BioInfoProject.Shared.DTOs;
using BioInfoProject.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BioInfoProject.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpPost("dashboard")]
    [ProducesResponseType(typeof(DashboardStatisticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardStatisticsDto>> GetDashboard(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _analyticsService.GetDashboardStatisticsAsync(request, cancellationToken));
    }

    [HttpPost("time-series")]
    [ProducesResponseType(typeof(IReadOnlyList<TimeSeriesPointDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TimeSeriesPointDto>>> GetTimeSeries(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _analyticsService.GetTimeSeriesAsync(request, cancellationToken));
    }

    [HttpPost("by-country")]
    [ProducesResponseType(typeof(IReadOnlyList<CountrySummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CountrySummaryDto>>> GetByCountry(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _analyticsService.GetCountrySummariesAsync(request, cancellationToken));
    }

    [HttpPost("by-region")]
    [ProducesResponseType(typeof(IReadOnlyList<RegionSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RegionSummaryDto>>> GetByRegion(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _analyticsService.GetRegionSummariesAsync(request, cancellationToken));
    }
}
