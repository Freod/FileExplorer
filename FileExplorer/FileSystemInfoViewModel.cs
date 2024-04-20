using System;
using System.IO;
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

        public FileSystemInfo Model
        {
            get => _fileSystemInfo;
            set
            {
                if (_fileSystemInfo != value)
                {
                    _fileSystemInfo = value;
                    LastWriteTime = value.LastWriteTime;
                    OnPropertyChanged();
                }
            }
        }
    }
}