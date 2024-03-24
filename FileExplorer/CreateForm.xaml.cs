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
        public string FileName { get; set; }
        public bool IsFolder { get; set; }

        public CreateForm()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            FileName = txtName.Text;
            IsFolder = chkIsFolder.IsChecked ?? false;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        //private void ShowCreateForm()
        //{
        //    CreateForm createForm = new CreateForm();
        //    //createForm.Owner = this; // Ustawienie właściciela formularza (opcjonalne)
        //    createForm.ShowDialog();

        //    // Po zamknięciu formularza możesz sprawdzić, czy użytkownik kliknął OK i pobrać dane z formularza
        //    if (createForm.DialogResult == true)
        //    {
        //        string name = createForm.FileName;
        //        bool isFolder = createForm.IsFolder;
        //        // Tutaj możesz użyć pobranych danych, np. do tworzenia pliku lub folderu
        //    }
        //}
    }
}
