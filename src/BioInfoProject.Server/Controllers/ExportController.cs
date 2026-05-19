using BioInfoProject.Server.Services.Interfaces;
using BioInfoProject.Shared.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BioInfoProject.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;

    public ExportController(IExportService exportService)
    {
        _exportService = exportService;
    }

    [HttpPost("records")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportRecords(
        ExportRequest request,
        CancellationToken cancellationToken)
    {
        if (request.DatasetId <= 0)
        {
            return BadRequest("A valid dataset id is required.");
        }

        var content = await _exportService.ExportRecordsAsync(request, cancellationToken);
        var fileName = $"filtered_records_dataset_{request.DatasetId}_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

        return File(content, "text/csv", fileName);
    }
}
