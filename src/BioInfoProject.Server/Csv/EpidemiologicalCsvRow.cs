namespace BioInfoProject.Server.Csv;

public class EpidemiologicalCsvRow
{
    public string DateReported { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string WhoRegion { get; set; } = string.Empty;

    public string NewCases { get; set; } = string.Empty;

    public string CumulativeCases { get; set; } = string.Empty;

    public string NewDeaths { get; set; } = string.Empty;

    public string CumulativeDeaths { get; set; } = string.Empty;
}
