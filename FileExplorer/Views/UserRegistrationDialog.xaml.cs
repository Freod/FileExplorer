using System.Windows;
using FileExplorer.Managers;

namespace FileExplorer.Views;

public partial class UserRegistrationDialog : Window
{
    private readonly FileManager _fileManager;
    
    public string Username { get; private set; }
    public string Password { get; private set; }
    public string IpAddress { get; private set; }

    public UserRegistrationDialog()
    {
        InitializeComponent();
        var context = new ApplicationDbContext();
        _fileManager = new FileManager(context);
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        Username = UsernameTextBox.Text;
        Password = PasswordBox.Password;
        IpAddress = IpAddressTextBox.Text;
        
        _fileManager.RegisterRemoteUser(Username, Password, IpAddress);

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}