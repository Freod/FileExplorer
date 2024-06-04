using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Application = System.Windows.Application;
using Image = System.Windows.Controls.Image;

namespace FileExplorer.Converters;

public class ResourceKeyToImageSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || !(value is string resourceName))
            return DependencyProperty.UnsetValue;

        if (Application.Current.MainWindow.Resources[resourceName] is Image image)
            return image.Source;

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ImageSource imageSource)
        {
            var uri = new Uri(imageSource.ToString());
            var fileName = Path.GetFileNameWithoutExtension(uri.LocalPath);
            return fileName;
        }

        return null;
    }
}