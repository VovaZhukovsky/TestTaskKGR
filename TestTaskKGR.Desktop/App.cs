using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestTaskKGR.Desktop;

public class App : Application
{
    readonly MainWindow mainWindow;

    public App(MainWindow mainWindow)
    {
        this.mainWindow = mainWindow;
    }
    protected override void OnStartup(StartupEventArgs e)
    {
        mainWindow.Show();  
        base.OnStartup(e);
    }
}
