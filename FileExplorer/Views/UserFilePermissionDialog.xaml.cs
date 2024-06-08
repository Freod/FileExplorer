using System.Windows;
using FileExplorer.Managers;
using FileExplorer.Models.Entities;
using FileExplorer.ViewModels;

namespace FileExplorer.Views;

public partial class UserFilePermissionDialog : Window
{
    private readonly FileManager _fileManager;

    public List<UserFilePermissionViewModel> UserPermissions { get; set; }

    public UserFilePermissionDialog(List<UserFilePermission> permissions)
    {
        InitializeComponent();
        DataContext = this;
        LoadPermissions(permissions);
        var context = new ApplicationDbContext();
        _fileManager = new FileManager(context);
    }

    private void LoadPermissions(List<UserFilePermission> permissions)
    {
        UserPermissions = new List<UserFilePermissionViewModel>();
        foreach (var permission in permissions)
        {
            UserPermissions.Add(new UserFilePermissionViewModel(permission));
        }

        UsersDataGrid.ItemsSource = UserPermissions;
    }

    private void CanDownloadButton_Click(object sender, RoutedEventArgs e)
    {
        if (UsersDataGrid.SelectedItem is UserFilePermissionViewModel userFilePermissionViewModel)
        {
            _fileManager.ChangeDownloadPermission(userFilePermissionViewModel.UserId);
            userFilePermissionViewModel.CanDownload = !userFilePermissionViewModel.CanDownload;
        }
    }

    private void CanUploadButton_Click(object sender, RoutedEventArgs e)
    {
        if (UsersDataGrid.SelectedItem is UserFilePermissionViewModel userFilePermissionViewModel)
        {
            _fileManager.ChangeUploadPermission(userFilePermissionViewModel.UserId);
            userFilePermissionViewModel.CanUpload = !userFilePermissionViewModel.CanUpload;
        }
    }

    private void CanSendNotificationsButton_Click(object sender, RoutedEventArgs e)
    {
        if (UsersDataGrid.SelectedItem is UserFilePermissionViewModel userFilePermissionViewModel)
        {
            _fileManager.ChangeSendNotificationsPermission(userFilePermissionViewModel.UserId);
            userFilePermissionViewModel.CanSendNotifications = !userFilePermissionViewModel.CanSendNotifications;
        }
    }
}