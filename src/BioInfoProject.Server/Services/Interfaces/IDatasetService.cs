using BioInfoProject.Shared.DTOs;

namespace BioInfoProject.Server.Services.Interfaces;

public interface IDatasetService
{
    Task<IReadOnlyList<DatasetDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<DatasetDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
