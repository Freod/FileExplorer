namespace FileExplorer.Models.Entities;

public class Notification
{
    public int NotificationId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}