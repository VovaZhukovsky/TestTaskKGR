using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Model;
using Microsoft.Extensions.Http;
using TestTaskKGR.Desktop.Commands;
using TestTaskKGR.Desktop.UserControls.ViewModels;
using TestTaskKGR.ApiClient;
using Microsoft.Extensions.Configuration;

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
