using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FileExplorer
{
    public class DirectoryInfoViewModel : FileSystemInfoViewModel
    {
        public ObservableCollection<FileSystemInfoViewModel> Items { get; set; }
            = new ObservableCollection<FileSystemInfoViewModel>();
        
        private int _count;

        public int Count
        {
            get => _count;
            set
            {
                if (_count != value)
                {
                    _count = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isExpanded;

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }

        public Exception Exception { get; private set; }

        private string _initialPath;
        private FileSystemWatcher _watcher;

        public DirectoryInfoViewModel(ObservableRecipient owner) : base(owner)
        {
        }

        public bool Open(string path)
        {
            _initialPath = path;
            bool result = false;
            try
            {
                foreach (var dirName in Directory.GetDirectories(path))
                {
                    var dirInfo = new DirectoryInfo(dirName);
                    DirectoryInfoViewModel itemViewModel = new DirectoryInfoViewModel(this);
                    itemViewModel.Open(dirName);
                    itemViewModel.Model = dirInfo;
                    Items.Add(itemViewModel);
                }

                foreach (var fileName in Directory.GetFiles(path))
                {
                    var fileInfo = new FileInfo(fileName);
                    FileInfoViewModel itemViewModel = new FileInfoViewModel(this);
                    itemViewModel.Model = fileInfo;
                    Items.Add(itemViewModel);
                }

                InitializeWatcher(path);
                result = true;
            }
            catch (Exception ex)
            {
                Exception = ex;
            }

            return result;
        }

        private void InitializeWatcher(string path)
        {
            _watcher = new FileSystemWatcher(path);
            _watcher.Created += OnFileSystemChanged;
            _watcher.Renamed += OnFileSystemChanged;
            _watcher.Deleted += OnFileSystemChanged;
            _watcher.Changed += OnFileSystemChanged;
            _watcher.Error += OnFileSystemError;
            _watcher.EnableRaisingEvents = true;
        }

        private void OnFileSystemChanged(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => OnFileSystemChanged(e));
        }

        private void OnFileSystemChanged(FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    HandleFileSystemCreate(e.FullPath, e.Name);
                    break;
                case WatcherChangeTypes.Deleted:
                    HandleFileSystemDelete(e.FullPath);
                    break;
                case WatcherChangeTypes.Changed:
                    // Obsługa zmiany pliku, jeżeli jest to wymagane
                    break;
                case WatcherChangeTypes.Renamed:

                    var renamedEvent = e as RenamedEventArgs;
                    if (renamedEvent != null)
                    {
                        HandleFileSystemDelete(renamedEvent.OldFullPath);
                        HandleFileSystemCreate(renamedEvent.FullPath, renamedEvent.Name);
                    }

                    break;
            }
        }

        private void OnFileSystemError(object sender, ErrorEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show("Error occurred in FileSystemWatcher: " + e.GetException().Message);
            });
        }

        private void HandleFileSystemCreate(string fullPath, string name)
        {
            if (Directory.Exists(fullPath))
            {
                var dirInfo = new DirectoryInfo(fullPath);
                DirectoryInfoViewModel itemViewModel = new DirectoryInfoViewModel(this);
                itemViewModel.Open(fullPath);
                itemViewModel.Model = dirInfo;
                Items.Add(itemViewModel);
            }
            else if (File.Exists(fullPath))
            {
                var fileInfo = new FileInfo(fullPath);
                FileInfoViewModel itemViewModel = new FileInfoViewModel(this);
                itemViewModel.Model = fileInfo;
                Items.Add(itemViewModel);
            }
        }

        public void HandleFileSystemDelete(string fullPath)
        {
            Items.Remove(Items.FirstOrDefault(item => item.Model.FullName == fullPath));
        }
        
        public new DirectoryInfo Model
        {
            get => (DirectoryInfo)base.Model;
            set
            {
                if (base.Model != value)
                {
                    base.Model = value;
                    Count = value.GetFileSystemInfos().Length; // example of additional property logic
                    OnPropertyChanged();
                }
            }
        }
    }
}