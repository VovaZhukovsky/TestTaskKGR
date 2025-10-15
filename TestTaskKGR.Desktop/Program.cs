using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Model;

namespace TestTaskKGR.Desktop;

public class Program
{
    [STAThread]
    public static void Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<App>();
                services.AddSingleton<WpfLogger>();
                services.AddSingleton<SKPaintSurfaceBehavior>();
                services.AddSingleton<ConfidentTrasholdSliderBehavior>();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<StreamParams>();
                services.AddSingleton<CommonParams>();
            })
            .Build();
        var app = host.Services.GetService<App>();
        app?.Run();
    }
}
