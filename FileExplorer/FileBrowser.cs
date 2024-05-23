using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FileExplorer
{
    public class FileBrowser: ObservableRecipient
    {
        public DirectoryInfoViewModel Root { get; set; }

        public string Lang
        {
            get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName; }
            set
            {
                if (value!=null)
                    if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != value)
                    {
                        CultureInfo.CurrentUICulture = new CultureInfo(value);
                        OnPropertyChanged(nameof(Lang));
                    }
            }
        }

        public FileBrowser()
        {
            OnPropertyChanged(nameof(Lang));
        }

        public void OpenRoot(string path)
        {
            Root = new DirectoryInfoViewModel();
            Root.Open(path);
        }
    }
}