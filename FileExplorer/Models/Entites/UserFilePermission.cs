namespace FileExplorer.Models.Entities;

public class UserFilePermission
{
    public int UserId { get; set; }
    public User User { get; set; }

    public int FileId { get; set; }
    public File File { get; set; }

    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    
    public bool CanDownload { get; set; }
    public bool CanUpload { get; set; }
    public bool CanSendNotifications { get; set; }
}