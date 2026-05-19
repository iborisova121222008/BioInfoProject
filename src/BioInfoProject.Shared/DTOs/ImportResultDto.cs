namespace BioInfoProject.Shared.DTOs;

public class ImportResultDto
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public int? DatasetId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public int ImportedRows { get; set; }

    public int SkippedRows { get; set; }

    public List<string> Errors { get; set; } = new();
}
