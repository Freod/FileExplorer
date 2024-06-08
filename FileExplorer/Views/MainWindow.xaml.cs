using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FileExplorer.Managers;
using FileExplorer.Resources;
using FileExplorer.ViewModels;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace FileExplorer.Views;

public partial class MainWindow : Window
{
    private readonly FileBrowser _fileBrowser;

    public MainWindow()
    {
        InitializeComponent();
        _fileBrowser = new FileBrowser();
        _fileBrowser.PropertyChanged += FileBrowser_PropertyChanged;
        _fileBrowser.OnOpenFileRequest += FileBrowser_OnOpenFileRequest;
        TreeView.SelectedItemChanged += TreeView_SelectedItemChanged;
        DataContext = _fileBrowser;

        var context = new ApplicationDbContext();
        var fileManager = new FileManager(context);
        fileManager.InitializeDatabase();
    }

    private void MenuExit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void FileBrowser_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(FileBrowser.Lang))
            CultureResources.ChangeCulture(new CultureInfo(_fileBrowser.Lang));
    }

    private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (TreeView.SelectedItem is FileSystemInfoViewModel item)
        {
            AttributesTextBlock.Text = GetFileAttributes(item.Model.Attributes);
            TextPreviewScrollViewer.Content = "";
        }
    }

    private string GetFileAttributes(FileAttributes attributes)
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

    private void CreateMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var createForm = new CreateForm();
        var result = createForm.ShowDialog();

        if (result == true && TreeView.SelectedItem is FileSystemInfoViewModel item)
            try
            {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error writing file: " + ex.Message);
            }
    }

    private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (TreeView.SelectedItem is FileSystemInfoViewModel item)
            try
            {
                if (Directory.Exists(item.Model.FullName))
                {
                    MessageBox.Show("Folder");
                    File.SetAttributes(item.Model.FullName, FileAttributes.Normal);
                    Directory.Exists(item.Model.FullName);
                    Directory.Delete(item.Model.FullName, true);
                }
                else
                {
                    File.Delete(item.Model.FullName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading file: " + ex.Message);
            }
    }

    private void FileBrowser_OnOpenFileRequest(object sender, FileInfoViewModel viewModel)
    {
        var content = _fileBrowser.GetFileContent(viewModel);
        if (content is string text)
        {
            var textView = new TextBlock { Text = text };
            TextPreviewScrollViewer.Content = textView;
        }
    }
    
    private void RegisterUserButton_Click(object sender, RoutedEventArgs e)
    {
        var registrationDialog = new UserRegistrationDialog();
        registrationDialog.ShowDialog();
    }

    private void ManageUsersButton_Click(object sender, RoutedEventArgs e)
    {
        var managementDialog = new UserManagementDialog();
        managementDialog.ShowDialog();
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