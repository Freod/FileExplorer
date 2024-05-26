using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FileExplorer
{
    public class FileInfoViewModel : FileSystemInfoViewModel
    {
        private long _size;
        private string _extension;


        public long Size
        {
            get => _size;
            set
            {
                if (_size != value)
                {
                    _size = value;
                    OnPropertyChanged();
                }
            }
        }
        

        public string Extension
        {
            get => _extension;
            set
            {
                if (_extension != value)
                {
                    _extension = value;
                    OnPropertyChanged();
                }
            }
        }
        
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
        
        public new FileInfo Model
        {
            get => (FileInfo)base.Model;
            set
            {
                if (base.Model != value)
                {
                    base.Model = value;
                    Size = value.Length;
                    Extension = value.Extension;
                    OnPropertyChanged();
                }
            }
        }
    }
}