using System.Globalization;
using System.Windows.Data;

namespace FileExplorer.Resources
{
    public class CultureResources
    {
        private static ObjectDataProvider _provider;

        public static ObjectDataProvider ResourceProvider
        {
            get
            {
                if (_provider == null)
                    _provider = System.Windows.Application.Current.FindResource("Strings") as ObjectDataProvider;
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