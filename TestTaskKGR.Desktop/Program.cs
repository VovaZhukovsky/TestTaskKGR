using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestTaskKGR.Desktop.Implementations;
using WebcamDemo;

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
                services.AddSingleton<StreamRunner>();
                services.AddSingleton<MainWindow>();
            })
            .Build();
        var app = host.Services.GetService<App>();
        app?.Run();
    }
}
