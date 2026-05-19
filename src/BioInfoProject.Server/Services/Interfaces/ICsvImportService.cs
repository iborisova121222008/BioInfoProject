using BioInfoProject.Shared.DTOs;

namespace BioInfoProject.Server.Services.Interfaces;

public interface ICsvImportService
{
    Task<ImportResultDto> ImportAsync(IFormFile file, CancellationToken cancellationToken = default);
}
