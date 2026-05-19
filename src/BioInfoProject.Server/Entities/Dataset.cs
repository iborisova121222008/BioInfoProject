namespace BioInfoProject.Server.Entities;

public class Dataset
{
    public int Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string OriginalFileName { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }

    public int RowCount { get; set; }

    public int CountryCount { get; set; }

    public int RegionCount { get; set; }

    public DateOnly? MinDate { get; set; }

    public DateOnly? MaxDate { get; set; }

    public ICollection<EpidemiologicalRecord> Records { get; set; } = new List<EpidemiologicalRecord>();
}
