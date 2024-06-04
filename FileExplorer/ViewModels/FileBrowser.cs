using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using FileExplorer.Converters.Comparers;
using FileExplorer.Models;
using FileExplorer.Views;
using MessageBox = System.Windows.MessageBox;

namespace FileExplorer.ViewModels;

public class FileBrowser : ObservableRecipient
{
    private static readonly string[] TextFilesExtensions = { ".txt", ".ini", ".log" };
    private CancellationTokenSource _cancellationTokenSource;
    private bool _isSorting;
    private int _maxThreadId;
    private DirectoryInfoViewModel _root;
    private string _statusMessage;

    public FileBrowser()
    {
        OnPropertyChanged(nameof(Lang));
        OpenRootFolderCommand = new RelayCommand(OpenRootFolderExecuteAsync);
        SortRootFolderCommand = new RelayCommand(SortRootFolderExecuteAsync, CanSortRootFolderExecute);
        OpenFileCommand = new RelayCommand(OpenFileExecute, OpenFileCanExecute);
        CancelCommand = new RelayCommand(CancelSorting, parameter => IsSorting);
    }

    public RelayCommand OpenRootFolderCommand { get; private set; }

    public RelayCommand SortRootFolderCommand { get; private set; }

    public RelayCommand OpenFileCommand { get; private set; }

    public RelayCommand CancelCommand { get; }

    public DirectoryInfoViewModel Root
    {
        get => _root;
        set
        {
            if (_root != value)
            {
                if (_root != null) _root.PropertyChanged -= Root_PropertyChanged;

                _root = value;

                if (_root != null) _root.PropertyChanged += Root_PropertyChanged;

                OnPropertyChanged();
            }
        }
    }

    public string Lang
    {
        get => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        set
        {
            if (value != null && CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != value)
            {
                CultureInfo.CurrentUICulture = new CultureInfo(value);
                OnPropertyChanged();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage != value)
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsSorting
    {
        get => _isSorting;
        set
        {
            if (_isSorting != value)
            {
                _isSorting = value;
                OnPropertyChanged();
            }
        }
    }

    public event EventHandler<FileInfoViewModel> OnOpenFileRequest;

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
        if (TextFilesExtensions.Contains(extension)) return GetTextFileContent(viewModel);

        return null;
    }

    private async void OpenRootFolderExecuteAsync(object parameter)
    {
        var dlg = new FolderBrowserDialog { Description = Strings.Select_directory_to_open };
        if (dlg.ShowDialog() == DialogResult.OK)
            await Task.Factory.StartNew(() =>
            {
                var path = dlg.SelectedPath;
                OpenRoot(path);
                StatusMessage = Strings.Ready;
            });
    }

    private void Root_PropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == "StatusMessage" && sender is FileSystemInfoViewModel viewModel)
            StatusMessage = viewModel.StatusMessage;
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
                                    StatusMessage = Strings.Sorting_canceled;
                                else
                                    StatusMessage = Strings.Sorting_complete;

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

        var taskArray = new Task[directories.Count];
        for (var i = 0; i < directories.Count; i++)
        {
            var directory = directories[i];

            var currentThreadId = Thread.CurrentThread.ManagedThreadId;
            StatusMessage = $"{Strings.Sorting_on_thread} {currentThreadId}, {directory.Model.FullName}";
            Debug.WriteLine($"Sorting on thread {currentThreadId}: {directory.Model.FullName}");
            sortedItems.Add(directory);

            taskArray[i] = Task.Factory.StartNew(async () =>
            {
                var innerThreadId = Thread.CurrentThread.ManagedThreadId;
                Debug.WriteLine($"Sorting on thread {innerThreadId}: {directory.Model.FullName}");
                if (innerThreadId > _maxThreadId)
                {
                    _maxThreadId = innerThreadId;
                    StatusMessage = $"{Strings.New_max_thread_ID}: {_maxThreadId}";
                }

                if (directory.Items.Any()) await SortItems(directory.Items, sortingOptions);
                // }, TaskCreationOptions.LongRunning).Unwrap();
                // }, TaskCreationOptions.PreferFairness).Unwrap();
            }, _cancellationTokenSource.Token).Unwrap();
        }

        sortedItems.AddRange(files);

        for (var i = 0; i < sortedItems.Count; i++)
            if (items[i] != sortedItems[i])
            {
                var oldIndex = items.IndexOf(sortedItems[i]);
                if (oldIndex >= 0)
                    items.Move(oldIndex, i);
                else
                    items.Insert(i, sortedItems[i]);
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
        if (parameter is FileInfoViewModel fileInfoViewModel) OnOpenFileRequest?.Invoke(this, fileInfoViewModel);
    }

    private object GetTextFileContent(FileInfoViewModel viewModel)
    {
        if (viewModel.Model is FileInfo fileInfo)
            try
            {
                var content = File.ReadAllText(fileInfo.FullName);
                return content;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.File_reading_error}: " + ex.Message);
            }

        return string.Empty;
    }
}