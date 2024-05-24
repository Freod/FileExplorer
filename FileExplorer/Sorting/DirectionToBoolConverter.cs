using System;
using System.Globalization;
using System.Windows.Data;

namespace FileExplorer.Sorting
{
    public class DirectionToBoolConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null || !(value is Direction))
                return false;

            Direction sortOrder = (Direction)value;
            char targetChar = parameter.ToString()[0];

            return sortOrder.ToString()[0] == targetChar;
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
                    case 'A':
                        return Direction.Ascending;
                    case 'D':
                        return Direction.Descending;
                }
            }

            return null;
        }
    }
}