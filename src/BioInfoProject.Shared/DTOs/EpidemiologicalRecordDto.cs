namespace BioInfoProject.Shared.DTOs;

public class EpidemiologicalRecordDto
{
    public int Id { get; set; }

    public int DatasetId { get; set; }

    public DateOnly DateReported { get; set; }

    public string CountryCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string WhoRegion { get; set; } = string.Empty;

    public int NewCases { get; set; }

    public int CumulativeCases { get; set; }

    public int NewDeaths { get; set; }

    public int CumulativeDeaths { get; set; }
}
