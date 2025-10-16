using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Model;
using TestTaskKGR.ApiClient;
using Microsoft.Extensions.Configuration;
using TestTaskKGR.Desktop.Factory;

namespace TestTaskKGR.Desktop;

public class Program
{
    [STAThread]
    public static void Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context,services) =>
            {
                services.AddSingleton<App>();
                services.AddSingleton<WpfLogger>();
                services.AddSingleton<MainWindow>();
                services.AddTransient<StreamParams>();
                services.AddTransient<CommonParams>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<IStreamViewModelFactory, StreamViewModelFactory>();
                services.AddSingleton<IStreamControlViewModelFactory, StreamControlViewModelFactory>();
                services.AddHttpClient<TestTaskKGRApiClient>(client =>
                    {
                        client.BaseAddress = new Uri(context.Configuration["TestTaskKGRApiUri"]);
                    });
            })
            .Build();
        var app = host.Services.GetService<App>();
        app?.Run();
    }
}
