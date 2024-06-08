namespace FileExplorer.Models.Entities;

public class File
{
    public int FileId { get; set; }
    public string FileName { get; set; }
    public int CreatedByUserId { get; set; }
    public User CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    public string Title { get; set; }
    public string Creator { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string Publisher { get; set; }
    public string Contributor { get; set; }
    public DateTime? Date { get; set; }
    public string Type { get; set; }
    public string Format { get; set; }
    public string Identifier { get; set; }
    public string Source { get; set; }
    public string Language { get; set; }
    public string Relation { get; set; }
    public string Coverage { get; set; }
    public string Rights { get; set; }
    
    public ICollection<FileModificationHistory> ModificationHistory { get; set; }
    public ICollection<UserFilePermission> UserFilePermissions { get; set; }
}