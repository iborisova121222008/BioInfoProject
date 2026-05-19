using BioInfoProject.Server.Services.Interfaces;
using BioInfoProject.Shared.DTOs;
using BioInfoProject.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BioInfoProject.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecordsController : ControllerBase
{
    private readonly IRecordService _recordService;

    public RecordsController(IRecordService recordService)
    {
        _recordService = recordService;
    }

    [HttpPost("filter")]
    [ProducesResponseType(typeof(PagedResultDto<EpidemiologicalRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResultDto<EpidemiologicalRecordDto>>> Filter(
        RecordFilterRequest request,
        CancellationToken cancellationToken)
    {
        if (request.DatasetId <= 0)
        {
            return BadRequest("A valid dataset id is required.");
        }

        var result = await _recordService.GetFilteredAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("countries/{datasetId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<string>>> GetCountries(
        int datasetId,
        CancellationToken cancellationToken)
    {
        var countries = await _recordService.GetCountriesAsync(datasetId, cancellationToken);
        return Ok(countries);
    }

    [HttpGet("regions/{datasetId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<string>>> GetRegions(
        int datasetId,
        CancellationToken cancellationToken)
    {
        var regions = await _recordService.GetRegionsAsync(datasetId, cancellationToken);
        return Ok(regions);
    }
}
