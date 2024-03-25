using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "Select directory to open";

                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    PopulateTreeView(dialog.SelectedPath);
                }
            }
        }

        private void PopulateTreeView(string folderPath)
        {
            TreeViewItem rootNode = new TreeViewItem();
            rootNode.Header = folderPath;
            rootNode.Tag = folderPath;
            TreeView.Items.Clear();
            TreeView.Items.Add(rootNode);
            TreeView.SelectedItemChanged += treeView_SelectedItemChanged;
            //treeView.MouseRightButtonDown += CreateMenuItem_Click;
            LoadFolders(folderPath, rootNode);
        }

        private void LoadFolders(string path, TreeViewItem parentNode)
        {
            try
            {
                string[] subdirectories = Directory.GetDirectories(path);
                foreach (string subdirectory in subdirectories)
                {
                    TreeViewItem folderNode = new TreeViewItem();
                    folderNode.Header = Path.GetFileName(subdirectory);
                    folderNode.Tag = subdirectory;
                    parentNode.Items.Add(folderNode);
                    LoadFolders(subdirectory, folderNode); // Recursive call
                }

                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    TreeViewItem fileNode = new TreeViewItem();
                    fileNode.Header = Path.GetFileName(file);
                    fileNode.Tag = file;
                    parentNode.Items.Add(fileNode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading folders: " + ex.Message);
            }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (TreeView.SelectedItem is TreeViewItem item)
            {
                string filePath = item.Tag as string;

                if (filePath != null)
                {
                    try
                    {
                        FileAttributes attributes = File.GetAttributes(filePath);
                        string attributeString = ConvertAttributesToString(attributes);
                        AttributesTextBlock.Text = $"Attributes: {attributeString}";
                    }
                    catch (Exception ex)
                    {
                        AttributesTextBlock.Text = $"Error: {ex.Message}";
                    }

                    if (File.Exists(filePath))
                    {
                        try
                        {
                            using (var textReader = File.OpenText(filePath))
                            {
                                string text = textReader.ReadToEnd();
                                TextBlock.Text = text;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error reading file: " + ex.Message);
                        }
                    }
                    else
                    {
                        TextBlock.Text = "Folder";
                    }
                }
                else
                {
                    TextBlock.Text = "The selected item is not a file or does not exist.";
                }
            }
        }

        private void treeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (TreeView.SelectedItem is TreeViewItem item && item.Parent is ItemsControl parent)
            {
                string path = item.Tag as string;
                if (path != null)
                {
                    try
                    {
                        ContextMenu contextMenu = FindResource("TreeViewContextMenu") as ContextMenu;
                        if (contextMenu != null)
                        {
                            if (File.Exists(path))
                            {
                                MenuItem createMenuItem = contextMenu.Items.OfType<MenuItem>()
                                    .FirstOrDefault(m => m.Name == "CreateMenuItem");
                                if (createMenuItem != null)
                                {
                                    createMenuItem.Visibility = Visibility.Collapsed;
                                }
                            }
                            
                            contextMenu.PlacementTarget = sender as UIElement;
                            contextMenu.IsOpen = true;
                            e.Handled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading file: " + ex.Message);
                    }
                }
            }
        }

        private void CreateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CreateForm createForm = new CreateForm();
            bool? result = createForm.ShowDialog();

            if (result == true)
            {
                try
                {
                    if (TreeView.SelectedItem is TreeViewItem item)
                    {
                        string currentPath = item.Tag as string;
                        if (currentPath != null)
                        {
                            string name = createForm.FileOrFolderName;
                            bool isFolder = createForm.IsFolder;
                            bool isReadOnly = createForm.IsReadOnly;
                            bool isArchive = createForm.IsArchive;
                            bool isHidden = createForm.IsHidden;
                            bool isSystem = createForm.IsSystem;

                            FileAttributes attributes = default(FileAttributes);
                            if (isReadOnly) attributes |= FileAttributes.ReadOnly;
                            if (isArchive) attributes |= FileAttributes.Archive;
                            if (isHidden) attributes |= FileAttributes.Hidden;
                            if (isSystem) attributes |= FileAttributes.System;

                            string newItemPath = Path.Combine(currentPath, name);
                            item.Items.Add(name);
                            if (isFolder)
                            {
                                Directory.CreateDirectory(newItemPath);
                                new DirectoryInfo(newItemPath).Attributes |= attributes;
                            }
                            else
                            {
                                File.Create(newItemPath);
                                File.SetAttributes(newItemPath, attributes);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading file: " + ex.Message);
                }
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem is TreeViewItem item && item.Parent is ItemsControl parent)
            {
                string path = item.Tag as string;
                if (path != null)
                {
                    try
                    {
                        if (Directory.Exists(path))
                        {
                            MessageBox.Show("Folder");

                            File.SetAttributes(path, FileAttributes.Normal);

                            var x = Directory.Exists(path);
                            parent.Items.Remove(item);
                            Directory.Delete(path, true);
                        }
                        else
                        {
                            parent.Items.Remove(item);
                            File.Delete(path);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading file: " + ex.Message);
                    }
                }
            }
        }

        private string ConvertAttributesToString(FileAttributes attributes)
        {
            string result = "";

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