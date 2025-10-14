using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestTaskKGR.Desktop.Implementations;

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
                services.AddSingleton<MainWindow>();
                services.AddSingleton<RunDetection>();
            })
            .Build();
        var app = host.Services.GetService<App>();
        app?.Run();
    }
}
