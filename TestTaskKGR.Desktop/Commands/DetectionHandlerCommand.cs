using System.Windows.Input;
using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Interfaces;

namespace TestTaskKGR.Desktop.Commands;

public class DetectionHandlerCommand
{
    private ILogger _logger;
    private StreamParams _streamParams;
    public ICommand Detection { get; set; }

    public DetectionHandlerCommand(StreamParams streamParams, ILogger logger)
    {
        _streamParams = streamParams;
        _logger = logger;

        Detection = new CommandHandler()
        {
            Method = StartDetection,
            CanExecuteMethod = CanStartDetection
        };
    }
    private bool CanStartDetection(object parameter)
    {
        return true;
    }
    private void StartDetection(object parameter)
    {
        bool newDetectionStatus = bool.Parse(parameter as string);

        _streamParams.IsRunDetection = newDetectionStatus;

        if (_streamParams.IsRunDetection)
            _logger.Log($"Отслеживание объектов включено");
        else
            _logger.Log($"Отслеживание объектов отключено");
    }
}
