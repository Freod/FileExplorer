using CommunityToolkit.Mvvm.ComponentModel;

namespace FileExplorer
{
    public class FileBrowser: ObservableRecipient
    {
        public DirectoryInfoViewModel Root { get; set; }

        public void OpenRoot(string path)
        {
            Root = new DirectoryInfoViewModel();
            Root.Open(path);
        }
    }
}