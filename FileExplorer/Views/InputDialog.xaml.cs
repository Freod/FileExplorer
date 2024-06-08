using System.Windows;

namespace FileExplorer.Views;

public partial class InputDialog : Window
{
    public string Input { get; private set; }

    public InputDialog(string prompt, string defaultValue = "")
    {
        InitializeComponent();
        PromptLabel.Content = prompt;
        InputTextBox.Text = defaultValue;
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        Input = InputTextBox.Text;
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}