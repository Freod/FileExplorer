using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FileExplorer
{
    public class FileSystemInfoViewModel : ObservableRecipient
    {
        private DateTime _creationTime;
        private DateTime _creationTimeUtc;
        private DateTime _lastWriteTime;
        private DateTime _lastWriteTimeUtc;
        private DateTime _lastAccessTime;
        private DateTime _lastAccessTimeUtc;
        private FileSystemInfo _fileSystemInfo;
        private String _caption;
        private ImageSource _icon;

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

        public String Caption
        {
            get => _caption;
            set
            {
                SetProperty(ref _caption, value);
                OnPropertyChanged();
            }
        }

        public ImageSource Icon
        {
            get => _icon;
            set
            {
                SetProperty(ref _icon, value);
                OnPropertyChanged();
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
                    Icon = GetIconForFileType(value);
                    OnPropertyChanged();
                }
            }
        }

        private ImageSource GetIconForFileType(FileSystemInfo file)
        {
            if (file is FileInfo fileInfo)
            {
                string extension = fileInfo.Extension.ToLower();
                switch (extension)
                {
                    case ".txt":
                        return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/txt.png"));
                    case ".png":
                        return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/imagefile.png"));
                    default:
                        return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/file.png"));
                }
            }

            return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/file.png"));
        }
    }
}