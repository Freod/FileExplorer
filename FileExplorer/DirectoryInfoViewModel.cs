using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace FileExplorer
{
    public class DirectoryInfoViewModel : FileSystemInfoViewModel
    {
        private FileSystemWatcher _watcher;

        public ObservableCollection<FileSystemInfoViewModel> Items { get; }
            = new ObservableCollection<FileSystemInfoViewModel>();

        public bool Open(string path)
        {
            var result = false;
            try
            {
                foreach (var dirName in Directory.GetDirectories(path))
                {
                    var dirInfo = new DirectoryInfo(dirName);
                    var itemViewModel = new DirectoryInfoViewModel();
                    InitializeWatcher(path);
                    itemViewModel.Open(dirName); // Rekurencyjnie otwórz podkatalog
                    itemViewModel.Model = dirInfo;
                    Items.Add(itemViewModel);
                }

                foreach (var fileName in Directory.GetFiles(path))
                {
                    var fileInfo = new FileInfo(fileName);
                    var itemViewModel = new FileInfoViewModel();
                    itemViewModel.Model = fileInfo;
                    Items.Add(itemViewModel);
                }

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd ładowania katalogów składowych: {ex.Message}");
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
            _watcher.Error += Watcher_Error;
            _watcher.EnableRaisingEvents = true;
        }

        private void OnFileSystemChanged(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => OnFileSystemChangedInternal(e));
        }

        private void OnFileSystemChangedInternal(FileSystemEventArgs e)
        {
            Console.WriteLine();
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    HandleCreate(e);
                    break;
                case WatcherChangeTypes.Renamed:
                    if (e is RenamedEventArgs renamedArgs)
                    {
                        var itemToRename = Items.FirstOrDefault(item => item.Model.FullName == renamedArgs.OldFullPath);
                        if (itemToRename != null) itemToRename.Model = new DirectoryInfo(renamedArgs.FullPath);
                    }

                    break;
                case WatcherChangeTypes.Deleted:
                    var itemToRemove = Items.FirstOrDefault(item => item.Model.FullName == e.FullPath);
                    // itemToRemove.
                    // var parentPath = Directory.GetParent(e.FullPath);
                    // var parent = Items.FirstOrDefault(item => item.Model == parentPath);
                    // string parentDirectory = Path.GetDirectoryName(e.FullPath);
                    // var parentItem = Items.FirstOrDefault(item => item.Model.FullName == parentDirectory);

                    if (itemToRemove != null) Items.Remove(itemToRemove);
                    break;
                case WatcherChangeTypes.Changed:
                    // var itemToRefresh = Items.FirstOrDefault(item => item.Model.FullName == e.FullPath);
                    // if (itemToRefresh != null)
                    // {
                    // Aktualizuj element w kolekcji na podstawie nowych danych
                    // if (e is FileSystemInfoViewModel file)
                    // {
                    //     itemToRefresh.Model = file;
                    // }
                    // else if (e is DirectoryInfoViewModel directory)
                    // {
                    //     itemToRefresh.Model = directory;
                    // }
                    // }

                    break;
            }
        }

        private void HandleCreate(FileSystemEventArgs e)
        {
            var fullPath = e.FullPath;
            // if (parentDirectory != null)
            // {
            if (File.Exists(fullPath))
            {
                var newFile = new FileInfo(fullPath);
                var newItemViewModel = new FileInfoViewModel();
                newItemViewModel.Model = newFile;
                Items.Add(newItemViewModel);
            }
            else if (Directory.Exists(fullPath))
            {
                // Stwórz nowy widok modelu dla katalogu i dodaj go do katalogu nadrzędnego
                // var newDirectory = new DirectoryInfo(fullPath);
                // var newItemViewModel = new DirectoryInfoViewModel();
                // newItemViewModel.Model = newDirectory;
                // newItemViewModel.Open(fullPath); // Rekurencyjnie otwórz podkatalog
                // Items.Add(newItemViewModel);
            }
        }

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            // Obsługa błędu FileSystemWatcher
        }
    }
}