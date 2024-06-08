namespace FileExplorer.Models.Entities;

public class FileModificationHistory
{
    public int FileModificationHistoryId { get; set; }
    public int FileId { get; set; }
    public File File { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string ModifiedBy { get; set; }
    public string ModificationDetails { get; set; }
}