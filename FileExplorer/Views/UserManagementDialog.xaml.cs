using System.Windows;
using FileExplorer.Managers;
using FileExplorer.Models.Entities;

namespace FileExplorer.Views;

public partial class UserManagementDialog : Window
{
    private readonly FileManager _fileManager;

    public UserManagementDialog()
    {
        InitializeComponent();
        var context = new ApplicationDbContext();
        _fileManager = new FileManager(context);
        LoadUsers();
    }

    private void LoadUsers()
    {
        UsersDataGrid.ItemsSource = _fileManager.ListUsers();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new UserRegistrationDialog();
        if (dialog.ShowDialog() == true)
        {
            // _fileManager.RegisterRemoteUser(dialog.Username, dialog.Password, dialog.IpAddress);
            LoadUsers();
        }
    }

    private void EditIpButton_Click(object sender, RoutedEventArgs e)
    {
        if (UsersDataGrid.SelectedItem is User selectedUser)
        {
            var inputDialog = new InputDialog("Enter new IP address:", selectedUser.IpAddress);
            if (inputDialog.ShowDialog() == true)
            {
                _fileManager.ChangeUserIpAddress(selectedUser.Username, inputDialog.Input);
                LoadUsers();
            }
        }
    }

    private void RemoveButton_Click(object sender, RoutedEventArgs e)
    {
        if (UsersDataGrid.SelectedItem is User selectedUser)
        {
            _fileManager.RemoveUser(selectedUser.Username);
            LoadUsers();
        }
    }

    private void BlockUnblockButton_Click(object sender, RoutedEventArgs e)
    {
        if (UsersDataGrid.SelectedItem is User selectedUser)
        {
            _fileManager.BlockUnblockUser(selectedUser.Username);
            LoadUsers();
        }
    }
}