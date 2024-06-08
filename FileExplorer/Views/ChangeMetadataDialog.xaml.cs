using System.Windows;
using System.Windows.Input;
using FileExplorer.ViewModels;

namespace FileExplorer.Views;

public partial class ChangeMetadataDialog : Window
{
    public ChangeMetadataDialog()
    {
        InitializeComponent();
    }

    public MetadataViewModel Metadata { get; set; }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        Metadata = (MetadataViewModel)DataContext;
        DialogResult = true;
    }
}