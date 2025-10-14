using System.Windows.Input;
using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Interfaces;

namespace TestTaskKGR.Desktop.Commands;

public class DetectionHandlerCommand
{
    private ILogger _logger;
    private RunDetection _runDetection;
    public ICommand Detection { get; set; }

    public DetectionHandlerCommand(RunDetection runDetection, ILogger logger)
    {
        _runDetection = runDetection;
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

        _runDetection.Status = newDetectionStatus;

        if (_runDetection.Status)
            _logger.Log($"Отслеживание объектов включено");
        else
            _logger.Log($"Отслеживание объектов отключено");
    }
}
