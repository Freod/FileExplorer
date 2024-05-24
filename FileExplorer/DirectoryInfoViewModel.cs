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
        public ObservableCollection<FileSystemInfoViewModel> Items { get; set; }
            = new ObservableCollection<FileSystemInfoViewModel>();

        public Exception Exception { get; private set; }

        private string _initialPath;
        private FileSystemWatcher _watcher;

        public bool Open(string path)
        {
            _initialPath = path;
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
                DirectoryInfoViewModel itemViewModel = new DirectoryInfoViewModel();
                itemViewModel.Open(fullPath);
                itemViewModel.Model = dirInfo;
                Items.Add(itemViewModel);
            }
            else if (File.Exists(fullPath))
            {
                var fileInfo = new FileInfo(fullPath);
                FileInfoViewModel itemViewModel = new FileInfoViewModel();
                itemViewModel.Model = fileInfo;
                Items.Add(itemViewModel);
            }
        }

        public void HandleFileSystemDelete(string fullPath)
        {
            Items.Remove(Items.FirstOrDefault(item => item.Model.FullName == fullPath));
        }
    }
}