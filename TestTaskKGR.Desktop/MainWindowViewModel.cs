using TestTaskKGR.Desktop.Commands;
using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.UserControls.ViewModels;

namespace TestTaskKGR.Desktop;

public class MainWindowViewModel
{
    public Stream1ControlViewModel Stream1Control { get; set; }
    public Stream2ControlViewModel Stream2Control { get; set; }
    public SKPaintSurfaceBehavior SKPaintSurfaceBehavior { get; set; }
    public DetectionHandlerCommand DetectionHandlerCommand {  get; set; }
    public WpfLogger LogConsole { get; set; }
    public Stream1ViewModel Stream1ViewModel { get; set; }
    public Stream2ViewModel Stream2ViewModel { get; set; }
    public RunDetection RunDetection { get; set; }

    public MainWindowViewModel(
        WpfLogger wpfLogger, 
        SKPaintSurfaceBehavior sKPaintSurfaceBehavior,
        RunDetection runDetection)
    {
        RunDetection = runDetection;
        LogConsole = wpfLogger;
        DetectionHandlerCommand = new DetectionHandlerCommand(RunDetection, LogConsole);
        Stream1ViewModel = new Stream1ViewModel(RunDetection);
        Stream2ViewModel = new Stream2ViewModel(RunDetection);
        SKPaintSurfaceBehavior = sKPaintSurfaceBehavior;
        Stream1Control = new Stream1ControlViewModel(LogConsole, Stream1ViewModel);
        Stream2Control = new Stream2ControlViewModel(LogConsole, Stream2ViewModel);
        
    }
}