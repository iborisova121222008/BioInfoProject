namespace BioInfoProject.Shared.DTOs;

public class TimeSeriesPointDto
{
    public DateOnly Date { get; set; }

    public long NewCases { get; set; }

    public int CumulativeCases { get; set; }

    public long NewDeaths { get; set; }

    public int CumulativeDeaths { get; set; }
}
