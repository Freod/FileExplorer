using System;
using System.Collections.ObjectModel;
using System.IO;
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
            // switch (e.ChangeType)
            // {
            //     case WatcherChangeTypes.Created:
            //         break;
            //     case WatcherChangeTypes.Renamed:
            //         break;
            //     case WatcherChangeTypes.Deleted:
            //         break;
            //     case WatcherChangeTypes.Changed:
            //         break;
            // }
        }

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            // Obsługa błędu FileSystemWatcher
        }
    }
}