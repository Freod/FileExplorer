using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FileExplorer
{
    public class CultureResources
    {
        private static ObjectDataProvider _provider;

        public static ObjectDataProvider ResourceProvider
        {
            get
            {
                if (_provider == null)
                    _provider =
                        (ObjectDataProvider)Application.Current.FindResource("Strings");
                return _provider;
            }
        }

        public Strings GetStringsInstance()
        {
            return new Strings();
        }

        public static void ChangeCulture(CultureInfo culture)
        {
            ResourceProvider.Refresh();
        }
    }
}