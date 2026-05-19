namespace BioInfoProject.Shared.DTOs;

public class ComparisonSeriesDto
{
    public string Name { get; set; } = string.Empty;

    public IReadOnlyList<TimeSeriesPointDto> Points { get; set; } = Array.Empty<TimeSeriesPointDto>();
}
