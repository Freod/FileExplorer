using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            ////treeView.MouseRightButtonDown += CreateMenuItem_Click;
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
                }
                else
                {
                    TextBlock.Text = "The selected item is not a file or does not exist.";
                }
            }
        }

        private void treeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            CreateForm createForm = new CreateForm();
            createForm.Owner = this; // Ustawiamy właściciela formularza (opcjonalne)
            createForm.ShowDialog();

            if (createForm.DialogResult == true)
            {
                string name = createForm.FileName;
                bool isFolder = createForm.IsFolder;

                if (isFolder)
                {
                    Directory.CreateDirectory(name); // Tworzenie nowego folderu
                }
                else
                {
                    File.Create(name); // Tworzenie nowego pliku
                }

                // Komunikat informujący użytkownika o sukcesie
                MessageBox.Show("File or folder was created successfully.", "Success", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Tutaj możemy użyć pobranych danych, np. do tworzenia pliku lub folderu
                // Należy również dodać nowy element do drzewa reprezentujący utworzony plik lub folder
                TreeViewItem newItem = new TreeViewItem();
                newItem.Header = name;
                newItem.Tag = isFolder ? "Folder" : "File";

                if (TreeView.SelectedItem is TreeViewItem selectedFolder)
                {
                    selectedFolder.Items.Add(newItem); // Dodajemy nowy element jako dziecko wybranego folderu
                }
                else
                {
                    TreeView.Items
                        .Add(newItem); // Jeśli nie ma wybranego folderu, dodajemy element do głównego poziomu drzewa
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