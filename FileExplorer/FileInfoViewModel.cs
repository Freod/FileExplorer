namespace FileExplorer
{
    public class FileInfoViewModel : FileSystemInfoViewModel
    {
        public FileInfoViewModel()
        {
            OpenFileCommand = new RelayCommand(OpenFileCommandExecute, CanOpenFileCommandExecute);
        }

        public RelayCommand OpenFileCommand { get; set; }

        private void OpenFileCommandExecute(object parameter)
        {
            int y;
        }

        private bool CanOpenFileCommandExecute(object parameter)
        {
            return true;
        }
    }
}