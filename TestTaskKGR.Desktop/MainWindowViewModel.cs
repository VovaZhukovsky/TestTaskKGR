using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.UserControls.ViewModels;
using WebcamDemo;
using YoloDotNet.Handlers;

namespace TestTaskKGR.Desktop;

public class MainWindowViewModel
{
    public Stream1ControlViewModel Stream1Control { get; set; }
    public StreamRunner StreamRunner { get; set; }
    public Stream2ControlViewModel Stream2Control { get; set; }
    public WpfLogger LogConsole { get; set; }

    public MainWindowViewModel(MainWindow mainWindow, WpfLogger wpfLogger, StreamRunner streamRunner)
    {
        LogConsole = wpfLogger;
        StreamRunner = streamRunner;
        Stream1Control = new Stream1ControlViewModel(LogConsole, streamRunner);
        Stream2Control = new Stream2ControlViewModel(LogConsole, streamRunner);
    }
}