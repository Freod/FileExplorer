using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
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

        public RelayCommand OpenRootFolderCommand { get; private set; }

        public RelayCommand SortRootFolderCommand { get; private set; }

        public RelayCommand OpenFileCommand { get; private set; }

        public DirectoryInfoViewModel Root { get; set; }

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

        public FileBrowser()
        {
            OnPropertyChanged(nameof(Lang));
            OpenRootFolderCommand = new RelayCommand(OpenRootFolderExecute);
            SortRootFolderCommand = new RelayCommand(SortRootFolderExecute, CanSortRootFolderExecute);
            OpenFileCommand = new RelayCommand(OpenFileExecute, OpenFileCanExecute);
        }

        public void OpenRoot(string path)
        {
            Root = new DirectoryInfoViewModel(this);
            Root.Open(path);
            OnPropertyChanged(nameof(Root));
        }

        public void Sort(SortingOptions sortingOptions)
        {
            SortItems(Root.Items, sortingOptions);
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

        private void OpenRootFolderExecute(object parameter)
        {
            var dlg = new FolderBrowserDialog() { Description = Strings.Select_directory_to_open };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var path = dlg.SelectedPath;
                OpenRoot(path);
            }
        }

        private bool CanSortRootFolderExecute(object parameter)
        {
            return Root != null;
        }

        private void SortRootFolderExecute(object parameter)
        {
            if (Root != null)
            {
                var sortDialog = new SortDialog();
                var result = sortDialog.ShowDialog();

                if (result == true)
                {
                    var options = sortDialog.Options;
                    Sort(options);
                }
            }

            OnPropertyChanged(nameof(Root));
        }

        private void SortItems(ObservableCollection<FileSystemInfoViewModel> items, SortingOptions sortingOptions)
        {
            var directories = items.OfType<DirectoryInfoViewModel>().ToList();
            var files = items.OfType<FileInfoViewModel>().ToList();

            directories.Sort(new DirectoryInfoViewModelComparer(sortingOptions.SortBy, sortingOptions.Direction));
            files.Sort(new FileInfoViewModelComparer(sortingOptions.SortBy, sortingOptions.Direction));

            var sortedItems = new List<FileSystemInfoViewModel>();

            foreach (var directory in directories)
            {
                sortedItems.Add(directory);
                if (directory.Items.Any())
                {
                    SortItems(directory.Items, sortingOptions);
                }
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