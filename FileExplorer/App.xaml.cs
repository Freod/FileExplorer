using Microsoft.EntityFrameworkCore;
using Application = System.Windows.Application;

namespace FileExplorer;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        using (var db = new ApplicationDbContext())
        {
            db.Database.Migrate();
        }
    }
}