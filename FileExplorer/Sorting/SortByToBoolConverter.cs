using System;
using System.Globalization;
using System.Windows.Data;

namespace FileExplorer.Sorting
{
    public class SortByToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null || !(value is SortBy))
                return false;

            SortBy sortBy = (SortBy)value;
            char targetChar = parameter.ToString()[0];

            return sortBy.ToString()[0] == targetChar;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null || !(value is bool))
                return null;

            bool isChecked = (bool)value;
            char targetChar = parameter.ToString()[0];

            if (isChecked)
            {
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
            }

            return null;
        }
    }
}