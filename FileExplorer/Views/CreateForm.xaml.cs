using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace FileExplorer.Views;

/// <summary>
///     Interaction logic for CreateForm.xaml
/// </summary>
public partial class CreateForm : Window
{
    public CreateForm()
    {
        InitializeComponent();
    }

    public string FileOrFolderName { get; set; }
    public bool IsFolder { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsArchive { get; set; }
    public bool IsHidden { get; set; }
    public bool IsSystem { get; set; }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Name.Text))
        {
            MessageBox.Show("The name must be filled in");
        }
        else
        {
            FileOrFolderName = Name.Text;
            IsFolder = CheckIsFolder.IsChecked ?? false;
            IsReadOnly = ReadOnlyCheckBox.IsChecked ?? false;
            IsArchive = ArchiveCheckBox.IsChecked ?? false;
            IsHidden = HiddenCheckBox.IsChecked ?? false;
            IsSystem = SystemCheckBox.IsChecked ?? false;
            DialogResult = true;
            Close();
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}