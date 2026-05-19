namespace BioInfoProject.Shared.Requests;

public class ExportRequest : RecordFilterRequest
{
    public string ExportType { get; set; } = "records";
}
