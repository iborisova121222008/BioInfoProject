namespace BioInfoProject.Shared.Requests;

public class AnalyticsFilterRequest
{
    public int DatasetId { get; set; }

    public string? Country { get; set; }

    public string? WhoRegion { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}
