using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Interfaces;
using System.Windows.Input;
using System.Windows.Controls;

namespace TestTaskKGR.Desktop.Commands;

public class TrackingHandlerCommand
{
    private ILogger _logger;
    private StreamParams _streamParams;
    public ICommand Track { get; set; }

    public TrackingHandlerCommand(StreamParams streamParams, ILogger logger)
    {
        _logger = logger;

        Track = new CommandHandler()
        {
            Method = StartTrack,
            CanExecuteMethod = CanStartTrack
        };
        _streamParams = streamParams;
    }
    private bool CanStartTrack(object parameter)
    {
        return true;
    }
    private void StartTrack(object parameter)
    {
        CheckBox? CheckBox = parameter as CheckBox;

        _streamParams.IsTrackingEnabled = (bool)CheckBox.IsChecked;

        if (_streamParams.IsTrackingEnabled)
            _logger.Log($"Тракинг включен");
        else
            _logger.Log($"Тракинг отключен");
    }
}
