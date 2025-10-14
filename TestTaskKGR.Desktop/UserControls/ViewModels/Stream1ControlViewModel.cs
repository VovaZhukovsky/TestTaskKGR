using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TestTaskKGR.Desktop.Implementations;
using TestTaskKGR.Desktop.Interfaces;
using WebcamDemo;

namespace TestTaskKGR.Desktop.UserControls.ViewModels;

public class Stream1ControlViewModel : INotifyPropertyChanged
{
    private ILogger _logger;
    private StreamRunner _streamRunner;
    public event PropertyChangedEventHandler PropertyChanged;
    public ICommand StartStream1Command { get; set; }
    public ICommand StopStream1Command { get; set; }

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

    public Stream1ControlViewModel(ILogger logger, StreamRunner streamRunner)
    {
        _logger = logger;
        _streamRunner = streamRunner;
        StartStream1Command = new CommandHandler()
        {
            Method = StartStream,
            CanExecuteMethod = CanStartStream
        };
        StopStream1Command = new CommandHandler()
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
        //_streamRunner.
        _logger.Log($"Стрим {Url} запущен");
    }
    private void StopStream(object parameter)
    {
        _logger.Log($"Стрим {Url} остановлен");
    }



    
}