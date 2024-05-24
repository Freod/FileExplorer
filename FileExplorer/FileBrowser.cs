using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FileExplorer
{
    public class FileBrowser : ObservableRecipient
    {
        public DirectoryInfoViewModel Root { get; set; }

        public string Lang
        {
            get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName; }
            set
            {
                if (value != null)
                    if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != value)
                    {
                        CultureInfo.CurrentUICulture = new CultureInfo(value);
                        OnPropertyChanged();
                    }
            }
        }

        public FileBrowser()
        {
            // Root = new DirectoryInfoViewModel(Owner);
            OnPropertyChanged(nameof(Lang));
            OpenRootFolderCommand = new RelayCommand(OpenRootFolderExecute);
            SortRootFolderCommand = new RelayCommand(SortRootFolderExecute, CanSortRootFolderExecute);
            // SortRootFolderCommand = new RelayCommand(
            //     execute: SortRootFolder,
            //     canExecute: CanSortRootFolder
            // );
        }

        public void OpenRoot(string path)
        {
            Root = new DirectoryInfoViewModel();
            Root.Open(path);
            OnPropertyChanged(nameof(Root));
        }

        public RelayCommand OpenRootFolderCommand { get; private set; }

        public RelayCommand SortRootFolderCommand { get; private set; }

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
                }

                // Root.Items = new ObservableCollection<FileSystemInfoViewModel>(
                //     Root.Items.OrderBy(item => item.Model.GetType().Name).Reverse()
                // );

                // foreach (var item in Root.Items)
                // {
                //     
                // }
            }
            OnPropertyChanged(nameof(Root));
        }

        // private void SortFolder(FileSystemInfoViewModel folder)
        // {
        //     folder.Items = new ObservableCollection<FileSystemInfoViewModel>(
        //         folder.Items.OrderBy(item => item.Model.GetType().Name)
        //     );
        //     
        //     foreach (var item in folder.Items)
        //     {
        //         SortFolder(item.Model);
        //     }
        // }
    }
}