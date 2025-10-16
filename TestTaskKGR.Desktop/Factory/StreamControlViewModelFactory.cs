using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Model;
using TestTaskKGR.Desktop.UserControls.ViewModels;
using TestTaskKGR.ApiClient;

namespace TestTaskKGR.Desktop.Factory;
public interface IStreamControlViewModelFactory
{
    StreamControlViewModel GetStreamControl(StreamViewModel streamViewModel);
}

public class StreamControlViewModelFactory : IStreamControlViewModelFactory
{
    private readonly WpfLogger _logConsole;
    private readonly CommonParams _commonParams;

    public StreamControlViewModelFactory(
        WpfLogger logConsole,
        CommonParams commonParams)
    {
        _logConsole = logConsole;
        _commonParams = commonParams;
    }

    public StreamControlViewModel GetStreamControl(StreamViewModel streamViewModel) =>
        new StreamControlViewModel(_logConsole, streamViewModel, _commonParams);
}
