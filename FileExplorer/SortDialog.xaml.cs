using System.Windows;
using FileExplorer.Sorting;

namespace FileExplorer
{
    public partial class SortDialog : Window
    {
        public SortDialog()
        {
            InitializeComponent();
            Options = new SortingOptions();
            DataContext = Options;
        }
        
        public SortingOptions Options { get; set; }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}