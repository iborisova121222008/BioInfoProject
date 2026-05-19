using BioInfoProject.Server.Services.Interfaces;
using BioInfoProject.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BioInfoProject.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatasetsController : ControllerBase
{
    private readonly IDatasetService _datasetService;

    public DatasetsController(IDatasetService datasetService)
    {
        _datasetService = datasetService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DatasetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DatasetDto>>> GetAll(CancellationToken cancellationToken)
    {
        var datasets = await _datasetService.GetAllAsync(cancellationToken);
        return Ok(datasets);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DatasetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DatasetDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var dataset = await _datasetService.GetByIdAsync(id, cancellationToken);

        if (dataset is null)
        {
            return NotFound();
        }

        return Ok(dataset);
    }
}
