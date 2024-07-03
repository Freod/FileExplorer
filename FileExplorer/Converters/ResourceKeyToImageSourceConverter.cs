using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace FileExplorer.Converters
{
    public class ResourceKeyToImageSourceConverter: IValueConverter
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
                Uri uri = new Uri(imageSource.ToString());
                string fileName = System.IO.Path.GetFileNameWithoutExtension(uri.LocalPath);
                return fileName;
            }

            return null;
        }
    }
}