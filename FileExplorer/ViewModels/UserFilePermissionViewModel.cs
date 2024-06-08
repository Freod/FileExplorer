using CommunityToolkit.Mvvm.ComponentModel;
using FileExplorer.Models.Entities;

namespace FileExplorer.ViewModels;

public class UserFilePermissionViewModel : ObservableRecipient
{
    private int _userId;

    public int UserId
    {
        get { return _userId; }
        set
        {
            if (_userId != value)
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }
    }

    private string _username;

    public string Username
    {
        get { return _username; }
        set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
    }

    private string _ipAddress;

    public string IpAddress
    {
        get { return _ipAddress; }
        set
        {
            if (_ipAddress != value)
            {
                _ipAddress = value;
                OnPropertyChanged(nameof(IpAddress));
            }
        }
    }

    private bool _canDownload;

    public bool CanDownload
    {
        get { return _canDownload; }
        set
        {
            if (_canDownload != value)
            {
                _canDownload = value;
                OnPropertyChanged(nameof(CanDownload));
            }
        }
    }

    private bool _canUpload;

    public bool CanUpload
    {
        get { return _canUpload; }
        set
        {
            if (_canUpload != value)
            {
                _canUpload = value;
                OnPropertyChanged(nameof(CanUpload));
            }
        }
    }

    private bool _canSendNotifications;

    public bool CanSendNotifications
    {
        get { return _canSendNotifications; }
        set
        {
            if (_canSendNotifications != value)
            {
                _canSendNotifications = value;
                OnPropertyChanged(nameof(CanSendNotifications));
            }
        }
    }

    public UserFilePermissionViewModel(UserFilePermission userFilePermission)
    {
        UserId = userFilePermission.User.UserId;
        Username = userFilePermission.User.Username;
        IpAddress = userFilePermission.User.IpAddress;
        CanDownload = userFilePermission.CanDownload;
        CanUpload = userFilePermission.CanUpload;
        CanSendNotifications = userFilePermission.CanSendNotifications;
    }
}