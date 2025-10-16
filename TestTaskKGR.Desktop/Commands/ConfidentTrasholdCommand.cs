using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestTaskKGR.Desktop.Interfaces;
using System.Windows.Input;
using TestTaskKGR.Desktop.Implementations;
using SkiaSharp.Views.Desktop;
using System.Windows;

namespace TestTaskKGR.Desktop.Commands;

public class ConfidentTrasholdCommand
{
    private ILogger _logger;
    private StreamParams _streamParams;
    public ICommand Change { get; set; }

    public ConfidentTrasholdCommand(StreamParams streamParams, ILogger logger)
    {
        _streamParams = streamParams;
        _logger = logger;

        Change = new CommandHandler()
        {
            Method = ChangeConfidentTrashold,
            CanExecuteMethod = CanChangeConfidentTrashold
        };
    }

    private void ChangeConfidentTrashold(object parameter)
    {
        RoutedPropertyChangedEventArgs<double>? e = parameter as RoutedPropertyChangedEventArgs<double>;
        if (e != null)
        {
            _streamParams.ConfidenceThreshold = e.NewValue;

        }
        //_logger.Log($"Значение confidence изменено на {_streamParams.ConfidenceThreshold}");
    }
    private bool CanChangeConfidentTrashold(object parameter)
    {
        return true;
    }
}
