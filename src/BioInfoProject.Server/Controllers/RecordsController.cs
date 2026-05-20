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

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EpidemiologicalRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EpidemiologicalRecordDto>> Update(
        int id,
        UpdateRecordRequest request,
        CancellationToken cancellationToken)
    {
        if (HasInvalidText(request))
        {
            return BadRequest("Date, country code, country, and WHO region are required.");
        }

        if (HasNegativeValues(request))
        {
            return BadRequest("Cases and deaths cannot be negative.");
        }

        var updatedRecord = await _recordService.UpdateAsync(id, request, cancellationToken);

        if (updatedRecord is null)
        {
            return NotFound();
        }

        return Ok(updatedRecord);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _recordService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private static bool HasInvalidText(UpdateRecordRequest request)
    {
        return string.IsNullOrWhiteSpace(request.CountryCode) ||
            string.IsNullOrWhiteSpace(request.Country) ||
            string.IsNullOrWhiteSpace(request.WhoRegion);
    }

    private static bool HasNegativeValues(UpdateRecordRequest request)
    {
        return request.NewCases < 0 ||
            request.CumulativeCases < 0 ||
            request.NewDeaths < 0 ||
            request.CumulativeDeaths < 0;
    }
}
