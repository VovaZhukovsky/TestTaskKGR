using TestTaskKGR.Desktop.Commands;
using TestTaskKGR.Desktop.Factory;
using TestTaskKGR.Desktop.Implementations;
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
    

    public MainWindowViewModel(
        WpfLogger wpfLogger,
        StreamParams streamParams,
        IStreamViewModelFactory streamFactory,
        IStreamControlViewModelFactory streamControlFactory)
    {
        LogConsole = wpfLogger;
        ConfidentTrasholdCommand = new ConfidentTrasholdCommand(streamParams,LogConsole);
        DetectionHandlerCommand = new DetectionHandlerCommand(streamParams, LogConsole);
        FilterHandlerCommand = new FilterHandlerCommand(streamParams,LogConsole);
        TrackingHandlerCommand = new TrackingHandlerCommand(streamParams,LogConsole);
        Stream1ViewModel = streamFactory.GetStream();
        Stream2ViewModel = streamFactory.GetStream();
        Stream1Control = streamControlFactory.GetStreamControl(Stream1ViewModel);
        Stream2Control = streamControlFactory.GetStreamControl(Stream2ViewModel);


    }
}