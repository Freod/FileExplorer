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
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FileBrowser _fileBrowser;
        
        public MainWindow()
        {
            InitializeComponent();
            _fileBrowser = new FileBrowser();
            DataContext = _fileBrowser;
            _fileBrowser.PropertyChanged += FileExplorer_PropertyChanged;
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
            var dlg = new FolderBrowserDialog() { Description = "Select directory to open" };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var path = dlg.SelectedPath;
                // var fileBrowser = new FileBrowser();
                _fileBrowser.OpenRoot(path);
                // DataContext = fileBrowser;
            }
        }

        // private void MenuOpen_Click(object sender, RoutedEventArgs e)
        // {
        //     using (var dialog = new FolderBrowserDialog())
        //     {
        //         dialog.Description = "Select directory to open";
        //
        //         var result = dialog.ShowDialog();
        //
        //         if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
        //             PopulateTreeView(dialog.SelectedPath);
        //     }
        // }
        //
        // private void PopulateTreeView(string folderPath)
        // {
        //     var rootNode = new TreeViewItem();
        //     rootNode.Header = folderPath;
        //     rootNode.Tag = folderPath;
        //     TreeView.Items.Clear();
        //     TreeView.Items.Add(rootNode);
        //     TreeView.SelectedItemChanged += treeView_SelectedItemChanged;
        //     //treeView.MouseRightButtonDown += CreateMenuItem_Click;
        //     LoadFolders(folderPath, rootNode);
        // }
        //
        // private void LoadFolders(string path, TreeViewItem parentNode)
        // {
        //     try
        //     {
        //         var subdirectories = Directory.GetDirectories(path);
        //         foreach (var subdirectory in subdirectories)
        //         {
        //             var folderNode = new TreeViewItem();
        //             folderNode.Header = Path.GetFileName(subdirectory);
        //             folderNode.Tag = subdirectory;
        //             parentNode.Items.Add(folderNode);
        //             LoadFolders(subdirectory, folderNode); // Recursive call
        //         }
        //
        //         var files = Directory.GetFiles(path);
        //         foreach (var file in files)
        //         {
        //             var fileNode = new TreeViewItem();
        //             fileNode.Header = Path.GetFileName(file);
        //             fileNode.Tag = file;
        //             parentNode.Items.Add(fileNode);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         MessageBox.Show("Error loading folders: " + ex.Message);
        //     }
        // }
        //
        // private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        // {
        //     if (TreeView.SelectedItem is TreeViewItem item)
        //     {
        //         var filePath = item.Tag as string;
        //
        //         if (filePath != null)
        //         {
        //             try
        //             {
        //                 var attributes = File.GetAttributes(filePath);
        //                 var attributeString = ConvertAttributesToString(attributes);
        //                 AttributesTextBlock.Text = $"Attributes: {attributeString}";
        //             }
        //             catch (Exception ex)
        //             {
        //                 AttributesTextBlock.Text = $"Error: {ex.Message}";
        //             }
        //
        //             if (File.Exists(filePath))
        //                 try
        //                 {
        //                     using (var textReader = File.OpenText(filePath))
        //                     {
        //                         var text = textReader.ReadToEnd();
        //                         TextBlock.Text = text;
        //                     }
        //                 }
        //                 catch (Exception ex)
        //                 {
        //                     MessageBox.Show("Error reading file: " + ex.Message);
        //                 }
        //             else
        //                 TextBlock.Text = "Folder";
        //         }
        //         else
        //         {
        //             TextBlock.Text = "The selected item is not a file or does not exist.";
        //         }
        //     }
        // }
        //
        // private void treeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        // {
        //     if (TreeView.SelectedItem is TreeViewItem item && item.Parent is ItemsControl parent)
        //     {
        //         var path = item.Tag as string;
        //         if (path != null)
        //             try
        //             {
        //                 var contextMenu = FindResource("TreeViewContextMenu") as ContextMenu;
        //                 if (contextMenu != null)
        //                 {
        //                     if (File.Exists(path))
        //                     {
        //                         var createMenuItem = contextMenu.Items.OfType<MenuItem>()
        //                             .FirstOrDefault(m => m.Name == "CreateMenuItem");
        //                         if (createMenuItem != null) createMenuItem.Visibility = Visibility.Collapsed;
        //                     }
        //                     else
        //                     {
        //                         var createMenuItem = contextMenu.Items.OfType<MenuItem>()
        //                             .FirstOrDefault(m => m.Name == "CreateMenuItem");
        //                         if (createMenuItem != null) createMenuItem.Visibility = Visibility.Visible;
        //                     }
        //
        //                     contextMenu.PlacementTarget = sender as UIElement;
        //                     contextMenu.IsOpen = true;
        //                     e.Handled = true;
        //                 }
        //             }
        //             catch (Exception ex)
        //             {
        //                 MessageBox.Show("Error reading file: " + ex.Message);
        //             }
        //     }
        // }
        //
        // private void CreateMenuItem_Click(object sender, RoutedEventArgs e)
        // {
        //     var createForm = new CreateForm();
        //     var result = createForm.ShowDialog();
        //
        //     if (result == true)
        //         try
        //         {
        //             if (TreeView.SelectedItem is TreeViewItem item)
        //             {
        //                 var currentPath = item.Tag as string;
        //                 if (currentPath != null)
        //                 {
        //                     var name = createForm.FileOrFolderName;
        //                     var isFolder = createForm.IsFolder;
        //                     var isReadOnly = createForm.IsReadOnly;
        //                     var isArchive = createForm.IsArchive;
        //                     var isHidden = createForm.IsHidden;
        //                     var isSystem = createForm.IsSystem;
        //
        //                     var attributes = default(FileAttributes);
        //                     if (isReadOnly) attributes |= FileAttributes.ReadOnly;
        //                     if (isArchive) attributes |= FileAttributes.Archive;
        //                     if (isHidden) attributes |= FileAttributes.Hidden;
        //                     if (isSystem) attributes |= FileAttributes.System;
        //
        //                     var newItemPath = Path.Combine(currentPath, name);
        //
        //                     if (Directory.Exists(newItemPath) || File.Exists(newItemPath))
        //                         throw new InvalidOperationException("File or directory exits.");
        //
        //                     if (isFolder)
        //                     {
        //                         Directory.CreateDirectory(newItemPath);
        //                         new DirectoryInfo(newItemPath).Attributes |= attributes;
        //                     }
        //                     else
        //                     {
        //                         var file = File.Create(newItemPath);
        //                         file.Close();
        //                         File.SetAttributes(newItemPath, attributes);
        //                     }
        //
        //                     var folderNode = new TreeViewItem();
        //                     folderNode.Header = name;
        //                     folderNode.Tag = newItemPath;
        //                     item.Items.Add(folderNode);
        //                 }
        //             }
        //         }
        //         catch (Exception ex)
        //         {
        //             MessageBox.Show("Error writing file: " + ex.Message);
        //         }
        // }
        //
        // private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        // {
        //     if (TreeView.SelectedItem is TreeViewItem item && item.Parent is ItemsControl parent)
        //     {
        //         var path = item.Tag as string;
        //         if (path != null)
        //             try
        //             {
        //                 if (Directory.Exists(path))
        //                 {
        //                     MessageBox.Show("Folder");
        //
        //                     File.SetAttributes(path, FileAttributes.Normal);
        //
        //                     var x = Directory.Exists(path);
        //                     parent.Items.Remove(item);
        //                     Directory.Delete(path, true);
        //                 }
        //                 else
        //                 {
        //                     parent.Items.Remove(item);
        //                     File.Delete(path);
        //                 }
        //             }
        //             catch (Exception ex)
        //             {
        //                 MessageBox.Show("Error reading file: " + ex.Message);
        //             }
        //     }
        // }
        //
        // private string ConvertAttributesToString(FileAttributes attributes)
        // {
        //     var result = "";
        //
        //     if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
        //         result += "r";
        //     else
        //         result += "-";
        //
        //     if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
        //         result += "a";
        //     else
        //         result += "-";
        //
        //     if ((attributes & FileAttributes.System) == FileAttributes.System)
        //         result += "s";
        //     else
        //         result += "-";
        //
        //     if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
        //         result += "h";
        //     else
        //         result += "-";
        //
        //     return result;
        // }
    }
}