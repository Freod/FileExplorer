using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Application = System.Windows.Application;
using ContextMenu = System.Windows.Controls.ContextMenu;
using Exception = System.Exception;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace FileExplorer
{
    public partial class MainWindow : Window
    {
        private FileBrowser _fileBrowser;

        public MainWindow()
        {
            InitializeComponent();
            _fileBrowser = new FileBrowser();
            DataContext = _fileBrowser;
            _fileBrowser.PropertyChanged += FileExplorer_PropertyChanged;
            TreeView.SelectedItemChanged += TreeView_SelectedItemChanged;
            // TreeView.PreviewMouseRightButtonDown += TreeView_PreviewMouseRightButtonDown;
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void FileExplorer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FileBrowser.Lang))
            {
                CultureResources.ChangeCulture(new CultureInfo(_fileBrowser.Lang));
            }
        }

        private void OpenDirMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // var dlg = new FolderBrowserDialog() { Description = Strings.Select_directory_to_open };
            // if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            // {
            //     var path = dlg.SelectedPath;
            //     _fileBrowser.OpenRoot(path);
            // }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (TreeView.SelectedItem is FileSystemInfoViewModel item)
            {
                AttributesTextBlock.Text = GetFileAttributes(item.Model.Attributes);
                // TextBlock.Text = GetFileContent(item);
            }
        }

        public string GetFileAttributes(FileAttributes attributes)
        {
            try
            {
                var attributeString = ConvertAttributesToString(attributes);
                return $"{Strings.Attributes}: {attributeString}";
            }
            catch (Exception ex)
            {
                return $"{Strings.Error}: {ex.Message}";
            }
        }

        private string GetFileContent(FileSystemInfoViewModel selectedItem)
        {
            if (selectedItem.Model is FileInfo fileInfo)
            {
                try
                {
                    string content = File.ReadAllText(fileInfo.FullName);
                    return content;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{Strings.File_reading_error}: {ex.Message}");
                }
            }

            return string.Empty;
        }

        // private void TreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        // {
        //     if (TreeView.SelectedItem is FileSystemInfoViewModel item)
        //     {
        //         try
        //         {
        //             var contextMenu = FindResource("TreeViewContextMenu") as ContextMenu;
        //             if (contextMenu != null)
        //             {
        //                 if (item is DirectoryInfoViewModel)
        //                 {
        //                     var createMenuItem = contextMenu.Items.OfType<MenuItem>()
        //                         .FirstOrDefault(m => m.Name == "CreateMenuItem");
        //                     if (createMenuItem != null) createMenuItem.Visibility = Visibility.Visible;
        //                 }
        //                 else
        //                 {
        //                     var createMenuItem = contextMenu.Items.OfType<MenuItem>()
        //                         .FirstOrDefault(m => m.Name == "CreateMenuItem");
        //                     if (createMenuItem != null) createMenuItem.Visibility = Visibility.Collapsed;
        //                 }
        //
        //                 contextMenu.PlacementTarget = sender as UIElement;
        //                 contextMenu.IsOpen = true;
        //                 e.Handled = true;
        //             }
        //         }
        //         catch (Exception ex)
        //         {
        //             MessageBox.Show($"{Strings.File_reading_error}: {ex.Message}");
        //         }
        //     }
        // }

        private void CreateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var createForm = new CreateForm();
            var result = createForm.ShowDialog();

            if (result == true)
                try
                {
                    // if (TreeView.SelectedItem is TreeViewItem item)
                    if (TreeView.SelectedItem is FileSystemInfoViewModel item)
                    {
                        // var currentPath = item.Tag as string;
                        // if (currentPath != null)
                        // {
                        var name = createForm.FileOrFolderName;
                        var isFolder = createForm.IsFolder;
                        var isReadOnly = createForm.IsReadOnly;
                        var isArchive = createForm.IsArchive;
                        var isHidden = createForm.IsHidden;
                        var isSystem = createForm.IsSystem;

                        var attributes = default(FileAttributes);
                        if (isReadOnly) attributes |= FileAttributes.ReadOnly;
                        if (isArchive) attributes |= FileAttributes.Archive;
                        if (isHidden) attributes |= FileAttributes.Hidden;
                        if (isSystem) attributes |= FileAttributes.System;

                        var newItemPath = Path.Combine(item.Model.FullName, name);

                        if (Directory.Exists(newItemPath) || File.Exists(newItemPath))
                            throw new InvalidOperationException("File or directory exits.");

                        if (isFolder)
                        {
                            Directory.CreateDirectory(newItemPath);
                            new DirectoryInfo(newItemPath).Attributes |= attributes;
                        }
                        else
                        {
                            var file = File.Create(newItemPath);
                            file.Close();
                            File.SetAttributes(newItemPath, attributes);
                        }

                        // var folderNode = new TreeViewItem();
                        // folderNode.Header = name;
                        // folderNode.Tag = newItemPath;
                        // item.Items.Add(folderNode);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error writing file: " + ex.Message);
                }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem is FileSystemInfoViewModel item)
            {
                try
                {
                    if (Directory.Exists(item.Model.FullName))
                    {
                        MessageBox.Show("Folder");

                        File.SetAttributes(item.Model.FullName, FileAttributes.Normal);

                        // var x = 
                        Directory.Exists(item.Model.FullName);
                        // HandleFileSystemDelete(item.Model.FullName);
                        Directory.Delete(item.Model.FullName, true);
                    }
                    else
                    {
                        // parent.Items.Remove(item);
                        File.Delete(item.Model.FullName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading file: " + ex.Message);
                }
            }
        }

        private string ConvertAttributesToString(FileAttributes attributes)
        {
            var result = "";

            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                result += "r";
            else
                result += "-";

            if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
                result += "a";
            else
                result += "-";

            if ((attributes & FileAttributes.System) == FileAttributes.System)
                result += "s";
            else
                result += "-";

            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                result += "h";
            else
                result += "-";

            return result;
        }
    }
}