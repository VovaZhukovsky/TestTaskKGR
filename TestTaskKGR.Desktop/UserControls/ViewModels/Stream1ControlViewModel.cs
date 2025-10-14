using SkiaSharp.Views.WPF;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TestTaskKGR.Desktop.Commands;
using TestTaskKGR.Desktop.Interfaces;

namespace TestTaskKGR.Desktop.UserControls.ViewModels;

public class Stream1ControlViewModel : INotifyPropertyChanged
{
    private ILogger _logger;
    private Stream1ViewModel _streamViewModel;
    private CancellationTokenSource _cancellationTokenSource;
    private bool _streamRunning;
    public event PropertyChangedEventHandler PropertyChanged;
    public ICommand StartStreamCommand { get; set; }
    public ICommand StopStreamCommand { get; set; }

    private SKElement _streamFrame;
    public SKElement Stream1Frame
    {
        get => _streamFrame;
        set
        {
            _streamFrame = value;
            NotifyPropertyChanged();
        }
    }

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

   public Stream1ControlViewModel(ILogger logger, Stream1ViewModel streamViewModel)
    {
        _logger = logger;
        StartStreamCommand = new CommandHandler()
        {
            Method = StartStream,
            CanExecuteMethod = CanStartStream
        };
        StopStreamCommand = new CommandHandler()
        {
            Method = StopStream,
            CanExecuteMethod = CanStopStream
        };
        _streamViewModel = streamViewModel;
    }

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool CanStartStream(object parameter)
    {
        if (!string.IsNullOrEmpty(Url) && !_streamRunning)
        {
            return true;
        }
        return false;
    }
    private bool CanStopStream(object parameter)
    {
        if (!string.IsNullOrEmpty(Url) && _streamRunning)
        {
            return true;
        }
        return false;
    }
    private void StartStream(object parameter)
    {
         _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(() => _streamViewModel.StreamAsync(Url,_streamFrame, _cancellationTokenSource.Token));
        _streamRunning = true;
        _logger.Log($"Стрим {Url} запущен");  
    }
    private async void StopStream(object parameter)
    {
        _cancellationTokenSource?.Cancel();
        _streamRunning = false;
        _logger.Log($"Стрим {Url} остановлен");
    }
}