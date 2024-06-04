using System.Globalization;
using System.Windows.Data;
using Application = System.Windows.Application;

namespace FileExplorer.Resources;

public class CultureResources
{
    private static ObjectDataProvider _provider;

    public static ObjectDataProvider ResourceProvider
    {
        get
        {
            if (_provider == null)
                _provider = Application.Current.FindResource("Strings") as ObjectDataProvider;
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