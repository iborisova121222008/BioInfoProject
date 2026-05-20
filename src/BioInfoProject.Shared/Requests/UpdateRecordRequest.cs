namespace BioInfoProject.Shared.Requests;

public class UpdateRecordRequest
{
    public DateOnly DateReported { get; set; }

    public string CountryCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string WhoRegion { get; set; } = string.Empty;

    public int NewCases { get; set; }

    public int CumulativeCases { get; set; }

    public int NewDeaths { get; set; }

    public int CumulativeDeaths { get; set; }
}
