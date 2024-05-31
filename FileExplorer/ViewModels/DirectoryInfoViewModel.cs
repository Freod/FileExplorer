using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using FileExplorer.Helpers;

namespace FileExplorer.ViewModels
{
    public class DirectoryInfoViewModel : FileSystemInfoViewModel
    {
        private FileSystemWatcher _watcher;
        private int _count;

        public DispatchedObservableCollection<FileSystemInfoViewModel> Items { get; set; }
            = new DispatchedObservableCollection<FileSystemInfoViewModel>();

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

        public Exception Exception { get; private set; }

        public new DirectoryInfo Model
        {
            get => (DirectoryInfo)base.Model;
            set
            {
                if (base.Model != value)
                {
                    base.Model = value;
                    Count = value.GetFileSystemInfos().Length;
                    OnPropertyChanged();
                }
            }
        }
        
        public DirectoryInfoViewModel(ObservableRecipient owner) : base(owner)
        {
            Items.CollectionChanged += Items_CollectionChanged;
        }

        public bool Open(string path)
        {
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
                    StatusMessage = Strings.LoadingSubDirectory + ", " + dirName;

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
                    HandleFileSystemCreate(e.FullPath);
                    break;
                case WatcherChangeTypes.Deleted:
                    HandleFileSystemDelete(e.FullPath);
                    break;
                case WatcherChangeTypes.Changed:
                    break;
                case WatcherChangeTypes.Renamed:

                    var renamedEvent = e as RenamedEventArgs;
                    if (renamedEvent != null)
                    {
                        HandleFileSystemDelete(renamedEvent.OldFullPath);
                        HandleFileSystemCreate(renamedEvent.FullPath);
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

        private void HandleFileSystemCreate(string fullPath)
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

        private void HandleFileSystemDelete(string fullPath)
        {
            Items.Remove(Items.FirstOrDefault(item => item.Model.FullName == fullPath));
        }
        
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in args.NewItems.Cast<FileSystemInfoViewModel>())
                    {
                        item.PropertyChanged += Root_PropertyChanged;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in args.NewItems.Cast<FileSystemInfoViewModel>())
                    {
                        item.PropertyChanged -= Root_PropertyChanged;
                    }
                    break;
            }
        }

        private void Root_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "StatusMessage" && sender is FileSystemInfoViewModel viewModel)
                this.StatusMessage = viewModel.StatusMessage;
        }
    }
}