using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FileExplorer.ViewModels;

public class FileInfoViewModel : FileSystemInfoViewModel
{
    private string _extension;
    private long _size;

    public FileInfoViewModel(ObservableRecipient owner) : base(owner)
    {
        OpenFileCommand = new RelayCommand(OpenFileExecute, OpenFileCanExecute);
    }

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

    public RelayCommand OpenFileCommand { get; set; }

    private void OpenFileExecute(object parameter)
    {
        OwnerExplorer.OpenFileCommand.Execute(parameter);
    }

    private bool OpenFileCanExecute(object parameter)
    {
        return OwnerExplorer.OpenFileCommand.CanExecute(parameter);
    }
}