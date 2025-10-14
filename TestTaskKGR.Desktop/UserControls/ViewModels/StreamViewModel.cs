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
    private double _confidenceThreshold;
    private string _stream;
    public SKElement _element;
    private VideoCapture _capture;
    private CommonParams _common;
    private List<ObjectDetection> _roi;
    public ICommand UpdateStreamFrameCommand { get; set; }
    public StreamViewModel(ILogger logger, StreamParams streamParams, CommonParams common)
    {
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
        _element = element;
        _stream = stream;
        using var capture = new VideoCapture(_stream, VideoCapture.API.Ffmpeg);
        capture.Set(CapProp.Fps, _common.FPS);
        capture.Set(CapProp.FrameWidth, _common.StreamWidth);
        capture.Set(CapProp.FrameHeight, _common.StreamHeigth);
        using var mat = new Mat();
        using var bgraMat = new Mat();
        using var resizeMat = new Mat();

        while (!cancellationToken.IsCancellationRequested)
        {
            // Capture the current frame from the webcam
            capture.Read(mat);
            CvInvoke.Resize(mat, resizeMat, new System.Drawing.Size(640, 480), 0, 0, Inter.Linear);
            CvInvoke.CvtColor(resizeMat, bgraMat, ColorConversion.Bgr2Bgra);

            //Create an SKBitmap from the BGRA Mat for processing
            using var frame = SKImage.FromPixels(_imageInfo, bgraMat.DataPointer, (int)bgraMat.Step);
            _currentFrame = SKBitmap.FromImage(frame);
            if (_streamParams.IsRunDetection)
            {
                // Run object detection on the current frame
                var resultsBottle = _yoloBottle.RunObjectDetection(_currentFrame, 0.5, iou: 0.1);
                var resultsPhone = _yoloPhone.RunObjectDetection(_currentFrame, 0.2, iou: 0.1);
                var results = _yolo.RunObjectDetection(_currentFrame, 0.2, iou: 0.1);
                if (results.Count() != 0)
                {
                    await _common.FindPersonByRoleAsync(results, _common.SellerLabel, _common.SellerRoiStream1.BoundingBox);

                    await _common.FindPersonByRoleAsync(results, _common.CustomerLabel, _common.CustomerRoiStream1.BoundingBox);

                    foreach (var person in results.Where(r => r.Label.Name == "seller").ToList())
                    {
                        var personRoi = new ObjectDetection
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
                            Label = new LabelModel { Name = "seller_roi" },
                            Confidence = person.Confidence,
                            Id = person.Id
                        };
                        if(resultsBottle.Count > 0)
                        {
                            if(resultsBottle.Any(bottle => personRoi.BoundingBox.Contains(bottle.BoundingBox)))
                            {
                                _logger.Log($"Нарушение {person.Label.Name} пьет воду");
                            }
                        }
                        results.Add(personRoi);
                    }

                    //results = results.Where(r => r.Label.Name == "person").Where(p => p.BoundingBox.IntersectsWith(region2.BoundingBox)).ToList().Select(p => new ObjectDetection(){ BoundingBox = p.BoundingBox, Confidence = p.Confidence, Id = p.Id, Label = CustomerLabel, Tail = p.Tail}).ToList();
                    //var seller = persons.Where(p => p.BoundingBox.IntersectsWith(region.BoundingBox));

                }
                //if (_streamParams.IsFilterEnabled)
                //results = results.FilterLabels(["seller", "customer"]);  // Optionally filter results to include only specific classes (e.g., "person", "cat", "dog")
                if (_streamParams.IsTrackingEnabled)
                    results.Track(_sortTracker); // Optionally track objects using the SortTracker
                // Draw detection and tracking results on the current frame
                //results.Add(region);
                //results.Add(region2);
                //_currentFrame.Draw(regions);
                _currentFrame.Draw(results);
                _currentFrame.Draw(resultsPhone.FilterLabels(["phone", "cell", "cell_phone", "mobile", "smartphone"]));
                _currentFrame.Draw(resultsBottle.FilterLabels(["bottle"]));
                //_currentFrame.Draw(resultsPhone);
                //_currentFrame.Draw(resultsBottle);
            }
            // Update GUI
            await _dispatcher.InvokeAsync(() =>
            {
                _element.InvalidateVisual(); // Notify SKiaSharp to update the frame.
              
            });
        }
        
        await _dispatcher.InvokeAsync(() =>
        {
            _currentFrame.Dispose();
            _currentFrame = null;
            _element.InvalidateVisual(); // Notify SKiaSharp to update the frame.

        });
    }
    public void UpdateStreamFrame(object parameter)
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
    public bool CanUpdateStreamFrame(object parameter)
    {
        return true;
    }
}
