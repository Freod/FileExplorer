using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileExplorer
{
    public class DirectoryInfoViewModel : FileSystemInfoViewModel
    {
        public ObservableCollection<FileSystemInfoViewModel> Items { get; private set; }
            = new ObservableCollection<FileSystemInfoViewModel>();

        public Exception Exception { get; private set; }

        public bool Open(string path)
        {
            bool result = false;
            try
            {
                foreach (var dirName in Directory.GetDirectories(path))
                {
                    var dirInfo = new DirectoryInfo(dirName);
                    DirectoryInfoViewModel itemViewModel = new DirectoryInfoViewModel();
                    itemViewModel.Open(dirName);
                    itemViewModel.Model = dirInfo;
                    Items.Add(itemViewModel);
                }

                foreach (var fileName in Directory.GetFiles(path))
                {
                    var fileInfo = new FileInfo(fileName);
                    FileInfoViewModel itemViewModel = new FileInfoViewModel();
                    itemViewModel.Model = fileInfo;
                    Items.Add(itemViewModel);
                }

                result = true;
            }
            catch (Exception ex)
            {
                Exception = ex;
            }

            return result;
        }
    }
}