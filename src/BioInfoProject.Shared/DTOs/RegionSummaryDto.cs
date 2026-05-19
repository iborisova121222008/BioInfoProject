namespace BioInfoProject.Shared.DTOs;

public class RegionSummaryDto
{
    public string WhoRegion { get; set; } = string.Empty;

    public int CountryCount { get; set; }

    public long TotalNewCases { get; set; }

    public int MaxCumulativeCases { get; set; }

    public long TotalNewDeaths { get; set; }

    public int MaxCumulativeDeaths { get; set; }
}
