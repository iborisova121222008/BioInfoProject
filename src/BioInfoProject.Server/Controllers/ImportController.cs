using BioInfoProject.Server.Services.Interfaces;
using BioInfoProject.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BioInfoProject.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase
{
    private readonly ICsvImportService _csvImportService;

    public ImportController(ICsvImportService csvImportService)
    {
        _csvImportService = csvImportService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ImportResultDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImportResultDto>> Import(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null)
        {
            return BadRequest(new ImportResultDto
            {
                Success = false,
                Message = "No file was uploaded."
            });
        }

        var result = await _csvImportService.ImportAsync(file, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
