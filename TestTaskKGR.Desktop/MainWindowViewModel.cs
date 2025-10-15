using TestTaskKGR.Desktop.Commands;
using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Model;
using TestTaskKGR.Desktop.UserControls.ViewModels;

namespace TestTaskKGR.Desktop;

public class MainWindowViewModel
{
    public StreamControlViewModel Stream1Control { get; set; }
    public StreamControlViewModel Stream2Control { get; set; }
    public ConfidentTrasholdCommand ConfidentTrasholdCommand { get; set; }
    public DetectionHandlerCommand DetectionHandlerCommand {  get; set; }
    public FilterHandlerCommand FilterHandlerCommand { get; set; }
    public TrackingHandlerCommand TrackingHandlerCommand { get; set; }
    public WpfLogger LogConsole { get; set; }
    public StreamViewModel Stream1ViewModel { get; set; }
    public StreamViewModel Stream2ViewModel { get; set; }
    public StreamParams StreamParams { get; set; }
    

    public MainWindowViewModel(
        WpfLogger wpfLogger,
        StreamParams streamParams,
        CommonParams common)
    {
        StreamParams = streamParams;
        LogConsole = wpfLogger;

        ConfidentTrasholdCommand = new ConfidentTrasholdCommand(StreamParams, LogConsole);
        DetectionHandlerCommand = new DetectionHandlerCommand(StreamParams, LogConsole);
        FilterHandlerCommand = new FilterHandlerCommand(StreamParams, LogConsole);
        TrackingHandlerCommand = new TrackingHandlerCommand(StreamParams, LogConsole);
        Stream1ViewModel = new StreamViewModel(LogConsole,StreamParams, common);
        Stream2ViewModel = new StreamViewModel(LogConsole,StreamParams, common);
        Stream1Control = new StreamControlViewModel(LogConsole, Stream1ViewModel, common);
        Stream2Control = new StreamControlViewModel(LogConsole, Stream2ViewModel, common);


    }
}