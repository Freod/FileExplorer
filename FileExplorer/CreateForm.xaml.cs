using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Xml.Linq;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for CreateForm.xaml
    /// </summary>
    public partial class CreateForm : Window
    {
        public string FileOrFolderName { get; set; }
        public bool IsFolder { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsArchive { get; set; }
        public bool IsHidden { get; set; }
        public bool IsSystem { get; set; }

        public CreateForm()
        {
            InitializeComponent();
        }

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
}