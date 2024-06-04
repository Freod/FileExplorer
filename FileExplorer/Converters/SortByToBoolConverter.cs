using System.Globalization;
using System.Windows.Data;
using FileExplorer.Models;

namespace FileExplorer.Converters;

public class SortByToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null || !(value is SortBy))
            return false;

        var sortBy = (SortBy)value;
        var targetChar = parameter.ToString()[0];

        return sortBy.ToString()[0] == targetChar;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null || !(value is bool))
            return null;

        var isChecked = (bool)value;
        var targetChar = parameter.ToString()[0];

        if (isChecked)
            switch (targetChar)
            {
                case 'N':
                    return SortBy.Name;
                case 'E':
                    return SortBy.Extension;
                case 'S':
                    return SortBy.Size;
                case 'D':
                    return SortBy.Date;
            }

        return null;
    }
}