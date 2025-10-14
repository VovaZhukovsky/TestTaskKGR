using SkiaSharp;
using SkiaSharp.Views.WPF;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TestTaskKGR.Desktop.Commands;
using TestTaskKGR.Desktop.Interfaces;
using TestTaskKGR.Desktop.Model;

namespace TestTaskKGR.Desktop.UserControls.ViewModels;

public class StreamControlViewModel : INotifyPropertyChanged
{
    private ILogger _logger;
    private StreamViewModel _streamViewModel;
    private CancellationTokenSource _cancellationTokenSource;
    private bool _streamRunning;
    public event PropertyChangedEventHandler PropertyChanged;
    private CommonParams _common;
    public ICommand StartStreamCommand { get; set; }
    public ICommand StopStreamCommand { get; set; }
    private SKRect _rect;
    public SKRect Rect
    {
        get => _rect;
        set
        {
            _rect = value;
            NotifyPropertyChanged();
        }
    }
    private SKBitmap _currentFrame;
    public SKBitmap CurrentFrame
    {
        get => _currentFrame;
        set
        {
            _currentFrame = value;
            NotifyPropertyChanged();
        }
    }

    private SKElement _streamFrame;
    public SKElement StreamFrame
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

   public StreamControlViewModel(ILogger logger, StreamViewModel streamViewModel, CommonParams common)
    {
        _common = common;
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
        _currentFrame = new SKBitmap(_common.StreamWidth, _common.StreamHeigth);
        _rect = new SKRect(0, 0, _common.StreamWidth, bottom: _common.StreamHeigth);
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