using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FileExplorer.ViewModels;

public class FileSystemInfoViewModel : ObservableRecipient
{
    private string _caption;
    private DateTime _creationTime;
    private DateTime _creationTimeUtc;
    private FileSystemInfo _fileSystemInfo;
    private DateTime _lastAccessTime;
    private DateTime _lastAccessTimeUtc;
    private DateTime _lastWriteTime;
    private DateTime _lastWriteTimeUtc;
    private string _statusMessage;

    protected FileSystemInfoViewModel(ObservableRecipient owner)
    {
        Owner = owner;
    }

    public DateTime CreationTime
    {
        get => _creationTime;
        set
        {
            SetProperty(ref _creationTime, value);
            OnPropertyChanged();
        }
    }

    public DateTime CreationTimeUtc
    {
        get => _creationTimeUtc;
        set
        {
            SetProperty(ref _creationTimeUtc, value);
            OnPropertyChanged();
        }
    }

    public DateTime LastWriteTime
    {
        get => _lastWriteTime;
        set
        {
            SetProperty(ref _lastWriteTime, value);
            OnPropertyChanged();
        }
    }

    public DateTime LastWriteTimeUtc
    {
        get => _lastWriteTimeUtc;
        set
        {
            SetProperty(ref _lastWriteTimeUtc, value);
            OnPropertyChanged();
        }
    }

    public DateTime LastAccessTime
    {
        get => _lastAccessTime;
        set
        {
            SetProperty(ref _lastAccessTime, value);
            OnPropertyChanged();
        }
    }

    public DateTime LastAccessTimeUtc
    {
        get => _lastAccessTimeUtc;
        set
        {
            SetProperty(ref _lastAccessTimeUtc, value);
            OnPropertyChanged();
        }
    }

    public string Caption
    {
        get => _caption;
        set
        {
            SetProperty(ref _caption, value);
            OnPropertyChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage != value)
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public FileSystemInfo Model
    {
        get => _fileSystemInfo;
        set
        {
            if (_fileSystemInfo != value)
            {
                _fileSystemInfo = value;
                CreationTime = value.CreationTime;
                CreationTimeUtc = value.CreationTimeUtc;
                LastWriteTime = value.LastWriteTime;
                LastWriteTimeUtc = value.LastWriteTimeUtc;
                LastAccessTime = value.LastAccessTime;
                LastAccessTimeUtc = value.LastAccessTimeUtc;
                Caption = value.Name;
                OnPropertyChanged();
            }
        }
    }

    public string IconResourceName
    {
        get
        {
            if (Model != null)
            {
                var extension = Model.Extension.ToLower();
                switch (extension)
                {
                    case ".txt":
                        return "txtIcon";
                    case ".png":
                        return "imageIcon";
                    default:
                        return "defaultIcon";
                }
            }

            return "defaultIcon";
        }
    }

    protected FileBrowser OwnerExplorer
    {
        get
        {
            var owner = Owner;
            while (owner is DirectoryInfoViewModel ownerDirectory)
            {
                if (ownerDirectory.Owner is FileBrowser explorer)
                    return explorer;
                owner = ownerDirectory.Owner;
            }

            return null;
        }
    }

    private ObservableRecipient Owner { get; }
}