using BioInfoProject.Shared.Requests;

namespace BioInfoProject.Server.Services.Interfaces;

public interface IExportService
{
    Task<byte[]> ExportRecordsAsync(ExportRequest request, CancellationToken cancellationToken = default);
}
