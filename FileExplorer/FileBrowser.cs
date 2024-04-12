using System.Collections.Specialized;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FileExplorer
{
    public class FileBrowser : ObservableRecipient
    {
        public FileBrowser()
        {
            OnPropertyChanged(nameof(Lang));
        }

        public DirectoryInfoViewModel Root { get; set; }

        public string Lang
        {
            get => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
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

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void OpenRoot(string path)
        {
            Root = new DirectoryInfoViewModel();
            Root.Open(path);
        }
    }
}