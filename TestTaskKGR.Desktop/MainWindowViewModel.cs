using TestTaskKGR.ApiClient;
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
    private PersonHandler _personHandler;
    

    public MainWindowViewModel(
        WpfLogger wpfLogger,
        StreamParams streamParams,
        CommonParams commonParams,
        TestTaskKGRApiClient httpClient)
    {
        LogConsole = wpfLogger;
        ConfidentTrasholdCommand = new ConfidentTrasholdCommand(streamParams,LogConsole);
        DetectionHandlerCommand = new DetectionHandlerCommand(streamParams, LogConsole);
        FilterHandlerCommand = new FilterHandlerCommand(streamParams,LogConsole);
        TrackingHandlerCommand = new TrackingHandlerCommand(streamParams,LogConsole);
        Stream1ViewModel = new StreamViewModel(LogConsole,streamParams,commonParams,httpClient);
        Stream2ViewModel = new StreamViewModel(LogConsole,streamParams,commonParams,httpClient);
        Stream1Control = new StreamControlViewModel(LogConsole,Stream1ViewModel,commonParams);
        Stream2Control = new StreamControlViewModel(LogConsole,Stream2ViewModel,commonParams);


    }
}