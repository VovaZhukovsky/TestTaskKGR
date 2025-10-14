using System.Windows.Input;
using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Interfaces;
using System.Windows.Controls;

namespace TestTaskKGR.Desktop.Commands;

public class FilterHandlerCommand
{
    private ILogger _logger;
    private StreamParams _streamParams;
    public ICommand Filter { get; set; }

    public FilterHandlerCommand(StreamParams streamParams, ILogger logger)
    {
        _logger = logger;

        Filter = new CommandHandler()
        {
            Method = StartFilter,
            CanExecuteMethod = CanStartFilter
        };
        _streamParams = streamParams;
    }
    private bool CanStartFilter(object parameter)
    {
        return true;
    }
    private void StartFilter(object parameter)
    {
        CheckBox? CheckBox = parameter as CheckBox;

        _streamParams.IsFilterEnabled = (bool)CheckBox.IsChecked;

        if (_streamParams.IsFilterEnabled)
            _logger.Log($"Фильтр включен");
        else
            _logger.Log($"Фильтр отключен");
    }
}
