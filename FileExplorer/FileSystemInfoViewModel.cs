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
        // private string _parentPath;

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
                    // Path.value.FullName;
                    // ParentPath;
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

        // public string ParentPath
        // {
        //     get => _parentPath;
        //     set
        //     {
        //         if (_parentPath != value)
        //         {
        //             _parentPath = value;
        //             OnPropertyChanged();
        //         }
        //     }
        // }
    }
}