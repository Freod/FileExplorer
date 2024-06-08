namespace FileExplorer.Models.Entities;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string IpAddress { get; set; }
    public string Role { get; set; }
    public bool IsBlocked { get; set; }

    public ICollection<File> CreatedFiles { get; set; }
    public ICollection<UserFilePermission> UserFilePermissions { get; set; }
    public ICollection<Notification> Notifications { get; set; }
}