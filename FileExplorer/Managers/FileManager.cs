using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Security;
using FileExplorer.Models.Entities;
using FileExplorer.ViewModels;
using FileExplorer.Views;
using Microsoft.EntityFrameworkCore;
using File = FileExplorer.Models.Entities.File;

namespace FileExplorer.Managers;

public class FileManager
{
    private readonly ApplicationDbContext _context;

    public FileManager(ApplicationDbContext context)
    {
        _context = context;
    }

    public void InitializeDatabase()
    {
        var localUserName = Environment.UserName;

        var user = _context.Users.SingleOrDefault(u => u.Username == localUserName);
        if (user == null)
        {
            Debug.WriteLine("First time setup. Please enter a password for the local user.");

            var password = GetSecurePasswordFromUser();

            if (ValidateSystemPassword(localUserName, password))
            {
                RegisterHostUser(localUserName, password);
            }
            else
            {
                var invalidPasswordMessage = "Invalid system password. Initialization failed.";
                MessageBox.Show(invalidPasswordMessage);
                Debug.WriteLine(invalidPasswordMessage);
            }
        }
        else
        {
            Debug.WriteLine("Welcome back! Please enter your password.");
            var password = GetSecurePasswordFromUser();

            if (user.Password != password) // In real application, compare hashed passwords
            {
                Debug.WriteLine("Incorrect password. Please try again.");
            }
            else
            {
                Debug.WriteLine("Login successful.");
            }
        }
    }

    private string GetSecurePasswordFromUser()
    {
        var passwordDialog = new PasswordDialog();
        if (passwordDialog.ShowDialog() == true)
        {
            return passwordDialog.Password;
        }

        throw new SecurityException("Password entry was canceled.");
    }

    private bool ValidateSystemPassword(string username, string password)
    {
        using (var context = new PrincipalContext(ContextType.Machine))
        {
            return context.ValidateCredentials(username, password);
        }
    }

    private string GetLocalIPAddress()
    {
        return "127.0.0.1";
    }

    private void RegisterHostUser(string username, string password)
    {
        var user = new User
        {
            Username = username,
            Password = password,
            IpAddress = GetLocalIPAddress(),
            Role = "Host",
            IsBlocked = false
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        Debug.WriteLine("Host user registered successfully.");
    }

    public void RegisterRemoteUser(string username, string password, string ipAddress)
    {
        if (_context.Users.Any(u => u.Username == username))
        {
            MessageBox.Show("User with this username already exists.");
            Debug.WriteLine("User with this username already exists.");
            return;
        }

        var user = new User
        {
            Username = username,
            Password = password,
            IpAddress = ipAddress,
            Role = "User",
            IsBlocked = false
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        Debug.WriteLine("Remote user registered successfully.");
    }

    public List<User> ListUsers()
    {
        return _context.Users.ToList();
    }

    public void BlockUnblockUser(string username)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user != null)
        {
            user.IsBlocked = !user.IsBlocked;
            _context.SaveChanges();
            Debug.WriteLine($"User {username} block status has been change.");
        }
        else
        {
            Debug.WriteLine($"User {username} not found.");
        }
    }

    public void ChangeUserIpAddress(string username, string newIpAddress)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user != null)
        {
            user.IpAddress = newIpAddress;
            _context.SaveChanges();
            Debug.WriteLine($"IP address for user {username} has been updated to {newIpAddress}.");
        }
        else
        {
            Debug.WriteLine($"User {username} not found.");
        }
    }

    public void RemoveUser(string username)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
            Debug.WriteLine($"User {username} has been removed.");
        }
        else
        {
            Debug.WriteLine($"User {username} not found.");
        }
    }

    public MetadataViewModel GetFileMetadata(FileInfo fileInfo)
    {
        var existingFile = _context.Files.FirstOrDefault(f => f.FileName == fileInfo.FullName);
        if (existingFile != null)
        {
            return new MetadataViewModel(existingFile);
        }

        var localUserName = Environment.UserName;
        var user = _context.Users.SingleOrDefault(u => u.Username == localUserName);
        var newFile = new File
        {
            FileName = fileInfo.FullName,
            CreatedAt = fileInfo.CreationTime,
            Title = fileInfo.Name,
            CreatedByUserId = user.UserId,
            Creator = user.Username
        };
        return new MetadataViewModel(newFile);
    }

    public void AddOrUpdateFileMetadata(File file)
    {
        _context.Files.Add(file);
        _context.SaveChanges();
    }

    public List<UserFilePermission> GetUsersPermissionToFile(FileInfo fileInfo)
    {
        var existingFile = _context.Files.FirstOrDefault(f => f.FileName == fileInfo.FullName);
        if (existingFile == null)
        {
            var file = AddNewFileFromFileInfo(fileInfo);
            _context.Files.Add(file);
            _context.SaveChanges();
            existingFile = file;
        }

        var users = _context.Users.ToList();
        foreach (var user in users)
        {
            var permission = new UserFilePermission
            {
                UserId = user.UserId,
                FileId = existingFile.FileId,
                CanRead = true,
                CanWrite = true,
                CanDownload = true,
                CanUpload = true,
                CanSendNotifications = true
            };
            _context.UserFilePermissions.Add(permission);
            _context.SaveChanges();
        }

        return _context.UserFilePermissions
            .Include(ufp => ufp.User)
            .Where(ufp => ufp.FileId == existingFile.FileId)
            .ToList();
    }

    private File AddNewFileFromFileInfo(FileInfo fileInfo)
    {
        var localUserName = Environment.UserName;
        var user = _context.Users.SingleOrDefault(u => u.Username == localUserName);
        return new File
        {
            FileName = fileInfo.FullName,
            CreatedAt = fileInfo.CreationTime,
            Title = fileInfo.Name,
            CreatedByUserId = user.UserId,
            Creator = user.Username,
            Subject = string.Empty,
            Description = string.Empty,
            Publisher = string.Empty,
            Contributor = string.Empty,
            Date = DateTime.Now,
            Type = string.Empty,
            Format = string.Empty,
            Identifier = string.Empty,
            Source = string.Empty,
            Language = string.Empty,
            Relation = string.Empty,
            Coverage = string.Empty,
            Rights = string.Empty
        };
    }

    public void ChangeDownloadPermission(int userId)
    {
        var permission = _context.UserFilePermissions.FirstOrDefault(u => u.UserId == userId);
        if (permission != null)
        {
            permission.CanDownload = !permission.CanDownload;
            _context.SaveChanges();
        }
        else
        {
            Debug.WriteLine($"User {userId} not found.");
        }
    }

    public void ChangeUploadPermission(int userId)
    {
        var permission = _context.UserFilePermissions.FirstOrDefault(u => u.UserId == userId);
        if (permission != null)
        {
            permission.CanUpload = !permission.CanUpload;
            _context.SaveChanges();
        }
        else
        {
            Debug.WriteLine($"User {userId} not found.");
        }
    }

    public void ChangeSendNotificationsPermission(int userId)
    {
        var permission = _context.UserFilePermissions.FirstOrDefault(u => u.UserId == userId);
        if (permission != null)
        {
            permission.CanSendNotifications = !permission.CanSendNotifications;
            _context.SaveChanges();
        }
        else
        {
            Debug.WriteLine($"User {userId} not found.");
        }
    }

    public void SaveEventInDatabase(string fileName, string eventName)
    {
        var existingFile = _context.Files.FirstOrDefault(f => f.FileName == fileName);
        if (existingFile == null)
        {
            var localUserName = Environment.UserName;
            var user = _context.Users.SingleOrDefault(u => u.Username == localUserName);
            var newFile = new File
            {
                FileName = fileName,
                CreatedAt = DateTime.Now,
                Title = fileName,
                CreatedByUserId = user.UserId,
                Creator = user.Username,
                Subject = string.Empty,
                Description = string.Empty,
                Publisher = string.Empty,
                Contributor = string.Empty,
                Date = DateTime.Now,
                Type = string.Empty,
                Format = string.Empty,
                Identifier = string.Empty,
                Source = string.Empty,
                Language = string.Empty,
                Relation = string.Empty,
                Coverage = string.Empty,
                Rights = string.Empty
            };
            _context.Files.Add(newFile);
            _context.SaveChanges();
            existingFile = newFile;
        }

        var modificationHistory = new FileModificationHistory
        {
            FileId = existingFile.FileId,
            ModifiedAt = DateTime.Now,
            ModifiedBy = Environment.UserName,
            ModificationDetails = eventName
        };

        _context.Add(modificationHistory);
        _context.SaveChanges();
    }

    public void RenameToDatabase(RenamedEventArgs e)
    {
        string oldFilePath = e.OldFullPath;
        string newFilePath = e.FullPath;

        var existingFile = _context.Files.FirstOrDefault(f => f.FileName == oldFilePath);
        if (existingFile != null)
        {
            var modificationHistory = new FileModificationHistory
            {
                FileId = existingFile.FileId,
                ModifiedAt = DateTime.Now,
                ModifiedBy = Environment.UserName,
                ModificationDetails = $"Renamed from {Path.GetFileName(oldFilePath)} to {Path.GetFileName(newFilePath)}"
            };
            existingFile.FileName = newFilePath;
            _context.Add(modificationHistory);
            _context.SaveChanges();
        }
    }
}