using System.Diagnostics;
using System.IO;
using System.Reflection;
using FileExplorer.Models.Entities;
using Microsoft.EntityFrameworkCore;
using File = FileExplorer.Models.Entities.File;

namespace FileExplorer;

public class ApplicationDbContext : DbContext
{
    private const string FolderName = "FileExplorer";

    public DbSet<User> Users { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<FileModificationHistory> FileModificationHistories { get; set; }
    public DbSet<UserFilePermission> UserFilePermissions { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var path = Path.Combine(folder, FolderName, "database.db");

        if (!Directory.Exists(Path.Combine(folder, FolderName)))
        {
            Directory.CreateDirectory(Path.Combine(folder, FolderName));
        }
        Console.WriteLine($"Database path: {path}");
        optionsBuilder.UseSqlite($"Data Source={path}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserFilePermission>()
            .HasKey(ufp => new { ufp.UserId, ufp.FileId });

        modelBuilder.Entity<UserFilePermission>()
            .HasOne(ufp => ufp.User)
            .WithMany(u => u.UserFilePermissions)
            .HasForeignKey(ufp => ufp.UserId);

        modelBuilder.Entity<UserFilePermission>()
            .HasOne(ufp => ufp.File)
            .WithMany(f => f.UserFilePermissions)
            .HasForeignKey(ufp => ufp.FileId);

        modelBuilder.Entity<File>()
            .HasOne(f => f.CreatedBy)
            .WithMany(u => u.CreatedFiles)
            .HasForeignKey(f => f.CreatedByUserId);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId);
    }
}