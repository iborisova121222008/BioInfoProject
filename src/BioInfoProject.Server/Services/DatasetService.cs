using BioInfoProject.Server.Data;
using BioInfoProject.Server.Entities;
using BioInfoProject.Server.Services.Interfaces;
using BioInfoProject.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BioInfoProject.Server.Services;

public class DatasetService : IDatasetService
{
    private readonly BioInfoDbContext _dbContext;

    public DatasetService(BioInfoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<DatasetDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Datasets
            .AsNoTracking()
            .OrderByDescending(dataset => dataset.UploadedAt)
            .Select(dataset => MapToDto(dataset))
            .ToListAsync(cancellationToken);
    }

    public async Task<DatasetDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Datasets
            .AsNoTracking()
            .Where(dataset => dataset.Id == id)
            .Select(dataset => MapToDto(dataset))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static DatasetDto MapToDto(Dataset dataset)
    {
        return new DatasetDto
        {
            Id = dataset.Id,
            FileName = dataset.FileName,
            OriginalFileName = dataset.OriginalFileName,
            UploadedAt = dataset.UploadedAt,
            RowCount = dataset.RowCount,
            CountryCount = dataset.CountryCount,
            RegionCount = dataset.RegionCount,
            MinDate = dataset.MinDate,
            MaxDate = dataset.MaxDate
        };
    }
}
