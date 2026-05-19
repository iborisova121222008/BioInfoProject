using System.Net.Http.Json;
using BioInfoProject.Shared.DTOs;
using BioInfoProject.Shared.Requests;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BioInfoProject.Client.Services;

public class BioInfoApiClient
{
    private const long MaxUploadSize = 25 * 1024 * 1024;

    private readonly HttpClient _httpClient;

    public BioInfoApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ImportResultDto> ImportCsvAsync(IBrowserFile file, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        await using var fileStream = file.OpenReadStream(MaxUploadSize, cancellationToken);
        using var fileContent = new StreamContent(fileStream);

        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        var response = await _httpClient.PostAsync("api/import", content, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ImportResultDto>(cancellationToken);

        if (result is not null)
        {
            return result;
        }

        return new ImportResultDto
        {
            Success = false,
            Message = response.IsSuccessStatusCode
                ? "The import response could not be read."
                : "The CSV file could not be imported."
        };
    }

    public async Task<IReadOnlyList<DatasetDto>> GetDatasetsAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<IReadOnlyList<DatasetDto>>(
            "api/datasets",
            cancellationToken) ?? Array.Empty<DatasetDto>();
    }

    public async Task<PagedResultDto<EpidemiologicalRecordDto>?> GetFilteredRecordsAsync(
        RecordFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/records/filter", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PagedResultDto<EpidemiologicalRecordDto>>(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetCountriesAsync(
        int datasetId,
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<IReadOnlyList<string>>(
            $"api/records/countries/{datasetId}",
            cancellationToken) ?? Array.Empty<string>();
    }

    public async Task<IReadOnlyList<string>> GetRegionsAsync(
        int datasetId,
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<IReadOnlyList<string>>(
            $"api/records/regions/{datasetId}",
            cancellationToken) ?? Array.Empty<string>();
    }

    public async Task<DashboardStatisticsDto?> GetDashboardStatisticsAsync(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/analytics/dashboard", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DashboardStatisticsDto>(cancellationToken);
    }

    public async Task<IReadOnlyList<TimeSeriesPointDto>> GetTimeSeriesAsync(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/analytics/time-series", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IReadOnlyList<TimeSeriesPointDto>>(cancellationToken)
            ?? Array.Empty<TimeSeriesPointDto>();
    }

    public async Task<IReadOnlyList<CountrySummaryDto>> GetCountrySummariesAsync(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/analytics/by-country", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IReadOnlyList<CountrySummaryDto>>(cancellationToken)
            ?? Array.Empty<CountrySummaryDto>();
    }

    public async Task<IReadOnlyList<RegionSummaryDto>> GetRegionSummariesAsync(
        AnalyticsFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/analytics/by-region", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IReadOnlyList<RegionSummaryDto>>(cancellationToken)
            ?? Array.Empty<RegionSummaryDto>();
    }

    public async Task ExportRecordsAsync(
        ExportRequest request,
        IJSRuntime jsRuntime,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/export/records", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
            ?? $"filtered_records_dataset_{request.DatasetId}.csv";

        await jsRuntime.InvokeVoidAsync(
            "bioInfoDownloads.downloadFile",
            fileName,
            Convert.ToBase64String(bytes));
    }
}
