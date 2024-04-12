using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FileExplorer
{
    public class FileSystemInfoViewModel : ObservableRecipient
    {
        private string _caption;
        private FileSystemInfo _fileSystemInfo;
        private DateTime _lastWriteTime;

        public FileSystemInfo Model
        {
            get => _fileSystemInfo;
            set
            {
                if (_fileSystemInfo != value)
                {
                    _fileSystemInfo = value;
                    LastWriteTime = value.LastWriteTime;
                    Caption = value.Name;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime LastWriteTime
        {
            get => _lastWriteTime;
            set
            {
                if (_lastWriteTime != value)
                {
                    _lastWriteTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Caption
        {
            get => _caption;
            set
            {
                if (_caption != value)
                {
                    _caption = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}