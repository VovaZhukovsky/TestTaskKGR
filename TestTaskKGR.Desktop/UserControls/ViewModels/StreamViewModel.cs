using Emgu.CV.CvEnum;
using Emgu.CV;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using TestTaskKGR.Desktop.Commands;
using YoloDotNet.Core;
using YoloDotNet.Enums;
using YoloDotNet.Models;
using YoloDotNet.Trackers;
using YoloDotNet;
using YoloDotNet.Extensions;
using TestTaskKGR.Desktop.Implementations;
using OpenTK.Platform.Windows;
using TestTaskKGR.Desktop.Model;
using System.Windows.Controls;
using TestTaskKGR.Desktop.Interfaces;
using static Emgu.CV.Dai.OpenVino;
using System.Drawing;
using YoloDotNet.Video.Services;
using TestTaskKGR.ApiClient;
using YoloDotNet.Video;
using System.IO;

namespace TestTaskKGR.Desktop.UserControls.ViewModels;

public partial class StreamViewModel : INotifyPropertyChanged
{
    private ILogger _logger;
    public event PropertyChangedEventHandler PropertyChanged;
    private readonly Yolo _yolo = default!;
    private readonly Yolo _yoloPhone = default!;
    private readonly Yolo _yoloBottle = default!;
    private readonly SortTracker _sortTracker = default!;
    public StreamParams _streamParams;
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
    private Dispatcher _dispatcher = default!;
    private SKImageInfo _imageInfo = default!;
    private string _stream;
    public SKElement _element;
    private string _outputFolder = default!;
    private VideoCapture _capture;
    private CommonParams _common;
    private List<ObjectDetection> _roi;
    private DateTime _streamStartTime;
    private DateTime _firstMotionTime;
    private DateTime _lastMotionTime;
    private Dictionary<int, PointF> _lastPositions = new Dictionary<int, PointF>();
    public ICommand UpdateStreamFrameCommand { get; set; }
    private PersonHandler _personHandler;
    private TestTaskKGRApiClient _httpClient;
    public StreamViewModel(ILogger logger, StreamParams streamParams, CommonParams common, TestTaskKGRApiClient httpClient)
    {
        _httpClient = httpClient;
        _logger = logger;
        _personHandler = new PersonHandler(_logger,_httpClient);
        _common = common;
        _streamParams = streamParams;
        UpdateStreamFrameCommand = new CommandHandler()
        {
            Method = UpdateStreamFrame,
            CanExecuteMethod = CanUpdateStreamFrame
        };
        _sortTracker = new SortTracker(costThreshold: 0.5f, maxAge: 5, tailLength: 30);
        _yolo = new Yolo(new YoloOptions
        {
            OnnxModel = _common.OnnexMainModelPath,
            ExecutionProvider = new CpuExecutionProvider(),
            ImageResize = ImageResize.Proportional,
            SamplingOptions = new(SKFilterMode.Nearest, SKMipmapMode.None)
        });
        _yoloPhone = new Yolo(new YoloOptions
        {
            OnnxModel = _common.OnnexPhoneModelPath,
            ExecutionProvider = new CpuExecutionProvider(),
            ImageResize = ImageResize.Proportional,
            SamplingOptions = new(SKFilterMode.Nearest, SKMipmapMode.None)
        });
        _yoloBottle = new Yolo(new YoloOptions
        {
            OnnxModel = _common.OnnexBottleModelPath,
            ExecutionProvider = new CpuExecutionProvider(),
            ImageResize = ImageResize.Proportional,
            SamplingOptions = new(SKFilterMode.Nearest, SKMipmapMode.None)
        });
        _dispatcher = Dispatcher.CurrentDispatcher;
        _currentFrame = new SKBitmap(_common.StreamWidth, _common.StreamHeigth);
        _rect = new SKRect(0, 0, _common.StreamWidth, bottom: _common.StreamHeigth);
        _imageInfo = new SKImageInfo(_common.StreamWidth, _common.StreamHeigth, SKColorType.Bgra8888, SKAlphaType.Premul);
        _roi = new List<ObjectDetection>() { _common.SellerRoiStream1, _common.CustomerRoiStream1 };
    }

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public async Task StopStreamAsync()
    {
        _capture.Dispose();

    }
    public async Task StreamAsync(string stream, SKElement element, CancellationToken cancellationToken = default)
    {
        _streamStartTime = DateTime.Now;
        _element = element;
        _stream = stream;
        using var capture = new VideoCapture(_stream, VideoCapture.API.Ffmpeg);
        capture.Set(CapProp.Fps, _common.FPS);
        capture.Set(CapProp.FrameWidth, _common.StreamWidth);
        capture.Set(CapProp.FrameHeight, _common.StreamHeigth);
        using var mat = new Mat();
        using var bgraMat = new Mat();
        using var resizeMat = new Mat();

        // обработка видео

       /* _yolo.InitializeVideo(new VideoOptions
        {
            VideoInput = _stream,
            VideoOutput = Path.Combine(_outputFolder, "video_output.mp4"),
            FrameRate = FrameRate.AUTO,
            Width = 720,
            Height = -2,
            CompressionQuality = 30,
            VideoChunkDuration = 0,
            FrameInterval = 0
        });
        _yolo.StartVideoProcessing();*/
        // обработка видео

        while (!cancellationToken.IsCancellationRequested)
        {
            // Capture the current frame from the webcam
            capture.Read(mat);
            CvInvoke.Resize(mat, resizeMat, new System.Drawing.Size(_common.StreamWidth, _common.StreamHeigth), 0, 0, Inter.Linear);
            CvInvoke.CvtColor(resizeMat, bgraMat, ColorConversion.Bgr2Bgra);

            //Create an SKBitmap from the BGRA Mat for processing
            using var frame = SKImage.FromPixels(_imageInfo, bgraMat.DataPointer, (int)bgraMat.Step);
            _currentFrame = SKBitmap.FromImage(frame);
            if (_streamParams.IsRunDetection)
            {
                    // Run object detection on the current frame
                    var resultsBottle = _yoloBottle.RunObjectDetection(_currentFrame, _streamParams.ConfidenceThreshold, iou: 0.1);
                    var resultsPhone = _yoloPhone.RunObjectDetection(_currentFrame, _streamParams.ConfidenceThreshold, iou: 0.1);
                    var results = _yolo.RunObjectDetection(_currentFrame, _streamParams.ConfidenceThreshold, iou: 0.1);
                    if (results.Count() != 0)
                    {

                        await _personHandler.FindPersonByRoleAsync(results, _common.SellerLabel, _common.SellerRoiStream1.BoundingBox);

                        await _personHandler.FindPersonByRoleAsync(results, _common.CustomerLabel, _common.CustomerRoiStream1.BoundingBox);
                        var personsRoi = await _personHandler.GetPersonRoiAsync(results.FilterLabels(["seller", "customer"]));
                        results.AddRange(personsRoi);
                        if (resultsBottle.Count() != 0)
                        {
                            await _personHandler.FindBottleAsync(personsRoi, resultsBottle);
                        }
                        if (resultsPhone.Count() != 0)
                        {
                            if (personsRoi.FilterLabels(["customerRoi"]).Count() != 0)
                            {
                                await _personHandler.FindPhoneAsync(personsRoi.FilterLabels(["sellerRoi"]), resultsPhone);
                            }

                        }

                    }
                if (_streamParams.IsFilterEnabled)
                        results = results.FilterLabels(["seller", "customer"]);
                        resultsPhone = resultsPhone.FilterLabels(["phone"]);
                        resultsBottle = resultsBottle.FilterLabels(["bottle"]);
                    if (_streamParams.IsTrackingEnabled)
                    {
                        var tracked = results.Track(_sortTracker);                      
                    }
                results.Add(_common.SellerRoiStream1);
                results.Add(_common.CustomerRoiStream1);
                _currentFrame.Draw(results);
                    _currentFrame.Draw(resultsPhone.FilterLabels(["phone", "cell", "cell_phone", "mobile", "smartphone"]));
                _currentFrame.Draw(resultsBottle.FilterLabels(["bottle"]));
                    
            }
            // Update GUI
            await _dispatcher.InvokeAsync(() =>
            {
                _element.InvalidateVisual();
              
            });
        }
        
        await _dispatcher.InvokeAsync(() =>
        {
            _streamParams.IsRunDetection = false;
            _currentFrame.Dispose();
            _currentFrame = null;
            _yolo?.Dispose();
            _yoloBottle?.Dispose();
            _yoloPhone?.Dispose();
            _element.InvalidateVisual(); 
            _yolo?.Dispose();

        });
    }
    private void UpdateStreamFrame(object parameter)
    {
        SKPaintSurfaceEventArgs? e = parameter as SKPaintSurfaceEventArgs;
        if (e != null)
        {
            using var canvas = e.Surface.Canvas;
            if(_currentFrame != null)
            {
                canvas.DrawBitmap(_currentFrame, _rect);
                canvas.Flush();
            }
            else
            {
                canvas.Clear();
            }
                
        }
    }
    private bool CanUpdateStreamFrame(object parameter)
    {
        return true;
    }

    //ai//
    private PointF GetCenter(RectangleF rect)
    => new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

    private float Distance(PointF a, PointF b)
        => (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
}
