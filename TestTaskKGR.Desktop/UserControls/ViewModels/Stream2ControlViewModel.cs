using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Interfaces;
using WebcamDemo;

namespace TestTaskKGR.Desktop.UserControls.ViewModels;

public class Stream2ControlViewModel : INotifyPropertyChanged
{
    private ILogger _logger;
    public event PropertyChangedEventHandler PropertyChanged;
    public ICommand StartStream2Command { get; set; }
    public ICommand StopStream2Command { get; set; }

    private string _url;
    public string Url
    {
        get => _url;
        set
        {
            _url = value;
            NotifyPropertyChanged();
        }
    }

    public Stream2ControlViewModel(ILogger logger,StreamRunner streamRunner)
    {
        _logger = logger;

        StartStream2Command = new CommandHandler()
        {
            Method = StartStream,
            CanExecuteMethod = CanStartStream
        };
        StopStream2Command = new CommandHandler()
        {
            Method = StopStream,
            CanExecuteMethod = CanStopStream
        };
    }

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool CanStartStream(object parameter) => !string.IsNullOrEmpty(Url);
    private bool CanStopStream(object parameter) => !string.IsNullOrEmpty(Url);
    private void StartStream(object parameter)
    {
        _logger.Log($"Стрим {Url} запущен");
    }
    private void StopStream(object parameter)
    {
        _logger.Log($"Стрим {Url} остановлен");
    }



    
}