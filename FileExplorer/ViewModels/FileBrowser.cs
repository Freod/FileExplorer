using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using FileExplorer.Converters.Comparers;
using FileExplorer.Models;
using FileExplorer.Views;

namespace FileExplorer.ViewModels
{
    public class FileBrowser : ObservableRecipient
    {
        private static readonly string[] TextFilesExtensions = { ".txt", ".ini", ".log" };
        public event EventHandler<FileInfoViewModel> OnOpenFileRequest;
        private String _statusMessage;
        private DirectoryInfoViewModel _root;
        private int _maxThreadId = 0;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isSorting;

        public RelayCommand OpenRootFolderCommand { get; private set; }

        public RelayCommand SortRootFolderCommand { get; private set; }

        public RelayCommand OpenFileCommand { get; private set; }

        public RelayCommand CancelCommand { get; }

        public DirectoryInfoViewModel Root
        {
            get { return _root; }
            set
            {
                if (_root != value)
                {
                    if (_root != null)
                    {
                        _root.PropertyChanged -= Root_PropertyChanged;
                    }

                    _root = value;

                    if (_root != null)
                    {
                        _root.PropertyChanged += Root_PropertyChanged;
                    }

                    OnPropertyChanged(nameof(Root));
                }
            }
        }

        public string Lang
        {
            get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName; }
            set
            {
                if (value != null && CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != value)
                {
                    CultureInfo.CurrentUICulture = new CultureInfo(value);
                    OnPropertyChanged();
                }
            }
        }

        public String StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        public bool IsSorting
        {
            get { return _isSorting; }
            set
            {
                if (_isSorting != value)
                {
                    _isSorting = value;
                    OnPropertyChanged(nameof(IsSorting));
                }
            }
        }

        public FileBrowser()
        {
            OnPropertyChanged(nameof(Lang));
            OpenRootFolderCommand = new RelayCommand(OpenRootFolderExecuteAsync);
            SortRootFolderCommand = new RelayCommand(SortRootFolderExecuteAsync, CanSortRootFolderExecute);
            OpenFileCommand = new RelayCommand(OpenFileExecute, OpenFileCanExecute);
            CancelCommand = new RelayCommand(CancelSorting, (parameter) => IsSorting);
        }

        public void OpenRoot(string path)
        {
            Debug.WriteLine("OpenRoot");
            Root = new DirectoryInfoViewModel(this);
            Root.Open(path);
            OnPropertyChanged(nameof(Root));
        }

        public object GetFileContent(FileInfoViewModel viewModel)
        {
            var extension = viewModel.Model.Extension?.ToLower();
            if (TextFilesExtensions.Contains(extension))
            {
                return GetTextFileContent(viewModel);
            }

            return null;
        }

        private async void OpenRootFolderExecuteAsync(object parameter)
        {
            var dlg = new FolderBrowserDialog() { Description = Strings.Select_directory_to_open };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                await Task.Factory.StartNew(() =>
                {
                    var path = dlg.SelectedPath;
                    OpenRoot(path);
                    this.StatusMessage = Strings.Ready;
                });
            }
        }

        private void Root_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "StatusMessage" && sender is FileSystemInfoViewModel viewModel)
                this.StatusMessage = viewModel.StatusMessage;
        }

        private bool CanSortRootFolderExecute(object parameter)
        {
            return Root != null;
        }

        private void SortRootFolderExecuteAsync(object parameter)
        {
            if (Root != null)
            {
                var sortDialog = new SortDialog();
                var result = sortDialog.ShowDialog();

                if (result == true)
                {
                    var options = sortDialog.Options;
                    _cancellationTokenSource = new CancellationTokenSource();
                    IsSorting = true;
                    
                    var synchronizationContext = SynchronizationContext.Current;

                    Task.Factory.StartNew(() =>
                    {
                        SortItems(Root.Items, options)
                            .ContinueWith(task =>
                            {
                                synchronizationContext.Post(_ =>
                                {
                                    if (task.IsCanceled)
                                    {
                                        StatusMessage = Strings.Sorting_canceled;
                                    }
                                    else
                                    {
                                        StatusMessage = Strings.Sorting_complete;
                                    }

                                    IsSorting = false;
                                }, null);
                            });
                    }, _cancellationTokenSource.Token);
                }
            }

            OnPropertyChanged(nameof(Root));
        }

        private void CancelSorting(object parametr)
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task SortItems(ObservableCollection<FileSystemInfoViewModel> items, SortingOptions sortingOptions)
        {
            var directories = items.OfType<DirectoryInfoViewModel>().ToList();
            var files = items.OfType<FileInfoViewModel>().ToList();

            directories.Sort(new DirectoryInfoViewModelComparer(sortingOptions.SortBy, sortingOptions.Direction));
            files.Sort(new FileInfoViewModelComparer(sortingOptions.SortBy, sortingOptions.Direction));

            var sortedItems = new List<FileSystemInfoViewModel>();

            Task[] taskArray = new Task[directories.Count];
            for (int i = 0; i < directories.Count; i++)
            {
                var directory = directories[i];

                int currentThreadId = Thread.CurrentThread.ManagedThreadId;
                StatusMessage = $"{Strings.Sorting_on_thread} {currentThreadId}, {directory.Model.FullName}";
                Debug.WriteLine($"Sorting on thread {currentThreadId}: {directory.Model.FullName}");
                sortedItems.Add(directory);

                taskArray[i] = Task.Factory.StartNew(async () =>
                {
                    int innerThreadId = Thread.CurrentThread.ManagedThreadId;
                    Debug.WriteLine($"Sorting on thread {innerThreadId}: {directory.Model.FullName}");
                    if (innerThreadId > _maxThreadId)
                    {
                        _maxThreadId = innerThreadId;
                        StatusMessage = $"{Strings.New_max_thread_ID}: {_maxThreadId}";
                    }

                    if (directory.Items.Any())
                    {
                        await SortItems(directory.Items, sortingOptions);
                    }
                    // }, TaskCreationOptions.LongRunning).Unwrap();
                    // }, TaskCreationOptions.PreferFairness).Unwrap();
                }, _cancellationTokenSource.Token).Unwrap();
            }

            sortedItems.AddRange(files);

            for (int i = 0; i < sortedItems.Count; i++)
            {
                if (items[i] != sortedItems[i])
                {
                    int oldIndex = items.IndexOf(sortedItems[i]);
                    if (oldIndex >= 0)
                    {
                        items.Move(oldIndex, i);
                    }
                    else
                    {
                        items.Insert(i, sortedItems[i]);
                    }
                }
            }

            await Task.WhenAll(taskArray);
        }

        private bool OpenFileCanExecute(object parameter)
        {
            if (parameter is FileInfoViewModel viewModel)
            {
                var extension = viewModel.Model.Extension?.ToLower();
                return TextFilesExtensions.Contains(extension);
            }

            return false;
        }

        private void OpenFileExecute(object parameter)
        {
            if (parameter is FileInfoViewModel fileInfoViewModel)
            {
                OnOpenFileRequest?.Invoke(this, fileInfoViewModel);
            }
        }

        private object GetTextFileContent(FileInfoViewModel viewModel)
        {
            if (viewModel.Model is FileInfo fileInfo)
            {
                try
                {
                    string content = File.ReadAllText(fileInfo.FullName);
                    return content;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{Strings.File_reading_error}: " + ex.Message);
                }
            }

            return string.Empty;
        }
    }
}