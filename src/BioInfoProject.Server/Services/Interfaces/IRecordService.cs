using BioInfoProject.Shared.DTOs;
using BioInfoProject.Shared.Requests;

namespace BioInfoProject.Server.Services.Interfaces;

public interface IRecordService
{
    Task<PagedResultDto<EpidemiologicalRecordDto>> GetFilteredAsync(
        RecordFilterRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetCountriesAsync(int datasetId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRegionsAsync(int datasetId, CancellationToken cancellationToken = default);
}
