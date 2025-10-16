using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Model;
using TestTaskKGR.Desktop.UserControls.ViewModels;
using TestTaskKGR.ApiClient;

namespace TestTaskKGR.Desktop.Factory;
public interface IStreamViewModelFactory
{
    StreamViewModel GetStream();
}

public class StreamViewModelFactory : IStreamViewModelFactory
{
    private readonly WpfLogger _logConsole;
    private readonly StreamParams _streamParams;
    private readonly CommonParams _commonParams;
    private readonly TestTaskKGRApiClient _httpClient;

    public StreamViewModelFactory(
        WpfLogger logConsole,
        StreamParams streamParams,
        CommonParams commonParams,
        TestTaskKGRApiClient httpClient)
    {
        _logConsole = logConsole;
        _streamParams = streamParams;
        _commonParams = commonParams;
        _httpClient = httpClient;
    }

    public StreamViewModel GetStream() =>
        new StreamViewModel(_logConsole, _streamParams, _commonParams, _httpClient);
}
