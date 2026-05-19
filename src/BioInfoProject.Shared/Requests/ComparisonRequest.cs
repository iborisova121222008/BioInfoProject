namespace BioInfoProject.Shared.Requests;

public class ComparisonRequest : AnalyticsFilterRequest
{
    public IReadOnlyList<string> Countries { get; set; } = Array.Empty<string>();

    public IReadOnlyList<string> WhoRegions { get; set; } = Array.Empty<string>();
}
