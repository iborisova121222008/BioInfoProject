namespace BioInfoProject.Shared.DTOs;

public class CountrySummaryDto
{
    public string Country { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    public long TotalNewCases { get; set; }

    public int MaxCumulativeCases { get; set; }

    public long TotalNewDeaths { get; set; }

    public int MaxCumulativeDeaths { get; set; }
}
