namespace BioInfoProject.Shared.DTOs;

public class DashboardStatisticsDto
{
    public long TotalNewCases { get; set; }

    public int MaxCumulativeCases { get; set; }

    public long TotalNewDeaths { get; set; }

    public int MaxCumulativeDeaths { get; set; }

    public int CountryCount { get; set; }

    public int RegionCount { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}
