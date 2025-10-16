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
using TestTaskKGR.Desktop.Model;
using TestTaskKGR.Desktop.Interfaces;
using System.Drawing;
using TestTaskKGR.ApiClient;

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
    private TestTaskKGRApiClient _httpClient;
    public StreamViewModel(ILogger logger, StreamParams streamParams, CommonParams common, TestTaskKGRApiClient httpClient)
    {
        _httpClient = httpClient;
        _logger = logger;
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

                        await FindPersonByRoleAsync(results, _common.SellerLabel, _common.SellerRoiStream1.BoundingBox);

                        await FindPersonByRoleAsync(results, _common.CustomerLabel, _common.CustomerRoiStream1.BoundingBox);
                        var personsRoi = await GetPersonRoiAsync(results.FilterLabels(["seller", "customer"]));
                        results.AddRange(personsRoi);
                        if (resultsBottle.Count() != 0)
                        {
                            await FindBottleAsync(personsRoi, resultsBottle);
                        }
                        if (resultsPhone.Count() != 0)
                        {
                            if (personsRoi.FilterLabels(["customerRoi"]).Count() != 0)
                            {
                                await FindPhoneAsync(personsRoi.FilterLabels(["sellerRoi"]), resultsPhone);
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

    public async Task FindPersonByRoleAsync(List<ObjectDetection> results, LabelModel label, SKRectI boundinxbox)
    {
        var personsRoi = results.Where(r => r.Label.Name == "person").Where(p => p.BoundingBox.IntersectsWith(boundinxbox)).ToList();
        if (personsRoi.Count() > 0)
        {
            var persons = personsRoi.Select(p => new ObjectDetection() { BoundingBox = p.BoundingBox, Confidence = p.Confidence, Id = p.Id, Label = label, Tail = p.Tail }).ToList();

            foreach (var person in persons)
            {
                var index = results.FindIndex(i => i.Id == person.Id);

                if (index != -1)
                    results[index] = person;
            }
        };
    }
    public async Task<List<ObjectDetection>> GetPersonRoiAsync(List<ObjectDetection> persons)
    {
        List<ObjectDetection> personsRoi = new();
        foreach (var person in persons)
        {
            string RoiLabelName;
            switch (person.Label.Name)
            {
                case "seller":
                    {
                        RoiLabelName = "seller_roi";
                        break;
                    }
                default:
                    {
                        RoiLabelName = "customer_roi";
                        break;
                    }

            }
            personsRoi.Add(new ObjectDetection
            {
                BoundingBox = new SKRectI
                {
                    Location = new SKPointI
                    {
                        X = person.BoundingBox.Left - 40,
                        Y = person.BoundingBox.Top - 40
                    },
                    Size = new SKSizeI
                    {
                        Width = person.BoundingBox.Width + 80,  // 40 слева + 40 справа
                        Height = person.BoundingBox.Height + 80 // 40 сверху + 40 снизу
                    }
                },
                Label = new LabelModel { Name = RoiLabelName },
                Confidence = person.Confidence,
                Id = person.Id
            });
        }
        return personsRoi;
    }
    public async Task FindBottleAsync(List<ObjectDetection> personsRoi, List<ObjectDetection> results)
    {
        foreach (var roi in personsRoi)
        {
            if (results.Any(bottle => roi.BoundingBox.Contains(bottle.BoundingBox)))
            {
                await _httpClient.Violation.PostAsync(new ViewModel.ViolationViewModel()
                {
                    DateTime = DateTime.Now,
                    RoleId = (await _httpClient.Role.GetByNameAsync(roi.Label.Name)).Id,
                    TypeId = 1

                });
                _logger.Log($"Нарушение: {roi.Label.Name} пьет воду");
            }
        }
    }
    public async Task FindPhoneAsync(List<ObjectDetection> sellerRoi, List<ObjectDetection> results)
    {
        foreach (var roi in sellerRoi)
        {
            if (results.Any(phone => roi.BoundingBox.Contains(phone.BoundingBox)))
            {
                await _httpClient.Violation.PostAsync(new ViewModel.ViolationViewModel()
                {
                    DateTime = DateTime.Now,
                    RoleId = _httpClient.Role.GetByNameAsync(roi.Label.Name).Id,
                    TypeId = 2
                });
                _logger.Log($"Нарушение: {roi.Label.Name} пьет воду");
            }
        }
    }
}
