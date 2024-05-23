using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace FileExplorer
{
    public class DirectoryInfoViewModel : FileSystemInfoViewModel
    {
        public ObservableCollection<FileSystemInfoViewModel> Items { get; private set; }
            = new ObservableCollection<FileSystemInfoViewModel>();

        public Exception Exception { get; private set; }

        private FileSystemWatcher _watcher;

        public bool Open(string path)
        {
            bool result = false;
            try
            {
                foreach (var dirName in Directory.GetDirectories(path))
                {
                    var dirInfo = new DirectoryInfo(dirName);
                    DirectoryInfoViewModel itemViewModel = new DirectoryInfoViewModel();
                    itemViewModel.Open(dirName);
                    itemViewModel.Model = dirInfo;
                    Items.Add(itemViewModel);
                }

                foreach (var fileName in Directory.GetFiles(path))
                {
                    var fileInfo = new FileInfo(fileName);
                    FileInfoViewModel itemViewModel = new FileInfoViewModel();
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

        // private void InitializeWatcher(string path)
        // {
        //     if (_watcher != null)
        //     {
        //         _watcher.EnableRaisingEvents = false;
        //         _watcher.Dispose();
        //     }
        //
        //     _watcher = new FileSystemWatcher(path)
        //     {
        //         NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
        //         IncludeSubdirectories = true
        //     };
        //
        //     _watcher.Changed += OnFileSystemChanged;
        //     _watcher.Created += OnFileSystemChanged;
        //     _watcher.Deleted += OnFileSystemChanged;
        //     _watcher.Renamed += OnFileSystemChanged;
        //     _watcher.Error += OnFileSystemError;
        //
        //     _watcher.EnableRaisingEvents = true;
        // }

        private void OnFileSystemChanged(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => OnFileSystemChanged(e));
        }

        // private async void OnFileSystemChanged(object sender, FileSystemEventArgs e)
        // {
        //     await Application.Current.Dispatcher.InvokeAsync(() => HandleFileSystemChanged(e));
        // }
        //
        // private void OnFileSystemRenamed(object sender, RenamedEventArgs e)
        // {
        //     Application.Current.Dispatcher.Invoke(() => HandleFileSystemRenamed(e));
        // }
        
        private void OnFileSystemChanged(FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    if (Directory.Exists(e.FullPath))
                    {
                        var dirInfo = new DirectoryInfo(e.Name);
                        DirectoryInfoViewModel itemViewModel = new DirectoryInfoViewModel();
                        itemViewModel.Open(e.FullPath);
                        itemViewModel.Model = dirInfo;
                        Items.Add(itemViewModel);
                    }
                    else if (File.Exists(e.FullPath))
                    {
                        var fileInfo = new FileInfo(e.Name);
                        FileInfoViewModel itemViewModel = new FileInfoViewModel();
                        itemViewModel.Model = fileInfo;
                        Items.Add(itemViewModel);
                    }
                    break;
                case WatcherChangeTypes.Deleted:
                    //TODO
                    Items.Remove(Items.FirstOrDefault(item => item.Model.FullName == e.FullPath));
                    break;
                case WatcherChangeTypes.Changed:
                    // Obsługa zmiany pliku, jeżeli jest to wymagane
                    break;
                case WatcherChangeTypes.Renamed:
                    var renamedEvent = e as RenamedEventArgs;
                    if (renamedEvent != null)
                    {
                        Items.Remove(Items.FirstOrDefault(item => item.Model.FullName == renamedEvent.OldFullPath));
                        if (Directory.Exists(e.FullPath))
                        {
                            var dirInfo = new DirectoryInfo(renamedEvent.Name);
                            DirectoryInfoViewModel itemViewModel = new DirectoryInfoViewModel();
                            itemViewModel.Open(renamedEvent.FullPath);
                            itemViewModel.Model = dirInfo;
                            Items.Add(itemViewModel);
                        }
                        else if (File.Exists(e.FullPath))
                        {
                            var fileInfo = new FileInfo(renamedEvent.Name);
                            FileInfoViewModel itemViewModel = new FileInfoViewModel();
                            itemViewModel.Model = fileInfo;
                            Items.Add(itemViewModel);
                        }
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
    }
}