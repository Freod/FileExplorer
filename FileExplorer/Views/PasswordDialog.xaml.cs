using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.Views;

public partial class PasswordDialog : Window
{
    public PasswordDialog()
    {
        InitializeComponent();
    }
    
    public string Password { get; private set; }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        Password = PasswordBox.Password;
        this.DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
    }
}