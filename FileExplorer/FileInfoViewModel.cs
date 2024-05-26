using CommunityToolkit.Mvvm.ComponentModel;

namespace FileExplorer
{
    public class FileInfoViewModel : FileSystemInfoViewModel
    {
        public FileInfoViewModel(ObservableRecipient owner): base(owner)
        {
            OpenFileCommand = new RelayCommand(OpenFileExecute, OpenFileCanExecute);
        }

        public RelayCommand OpenFileCommand { get; set; }

        private void OpenFileExecute(object parameter)
        {
            OwnerExplorer.OpenFileCommand.Execute(parameter);
        }

        private bool OpenFileCanExecute(object parameter)
        {
            // return true;
            return OwnerExplorer.OpenFileCommand.CanExecute(parameter);
        }
    }
}