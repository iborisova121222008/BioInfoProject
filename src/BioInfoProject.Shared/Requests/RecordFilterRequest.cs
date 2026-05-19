namespace BioInfoProject.Shared.Requests;

public class RecordFilterRequest
{
    public int DatasetId { get; set; }

    public string? Country { get; set; }

    public string? WhoRegion { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? SearchTerm { get; set; }

    public string SortBy { get; set; } = "DateReported";

    public string SortDirection { get; set; } = "asc";

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 25;
}
