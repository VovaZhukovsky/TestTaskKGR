// SPDX-License-Identifier: GPL-3.0-or-later
// Copyright (c) 2024-2025 Niklas Swärd
// https://github.com/NickSwardh/YoloDotNet

using Emgu.CV;
using Emgu.CV.CvEnum;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using YoloDotNet;
using YoloDotNet.Core;
using YoloDotNet.Enums;
using YoloDotNet.Extensions;
using YoloDotNet.Models;
using YoloDotNet.Trackers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TestTaskKGR.Desktop.Implementations;

namespace WebcamDemo;

public partial class StreamRunner : INotifyPropertyChanged
{
    #region Fields
    public event PropertyChangedEventHandler PropertyChanged;
    private readonly Yolo _yolo = default!;
    private readonly SortTracker _sortTracker = default!;
    private SKBitmap _currentFrame;
    public SKBitmap CurrentFrame
    {
        get => _currentFrame;
        set
        {
            CurrentFrame = value;
            NotifyPropertyChanged();
        }
    }
    private Dispatcher _dispatcher = default!;
    public bool _runDetection = false;
    private SKRect _rect;
    //private Stopwatch _stopwatch = default!;
    private SKImageInfo _imageInfo = default!;
    private bool _isTrackingEnabled;
    private bool _isFilteringEnabled;
    private double _confidenceThreshold;
    #endregion
    #region Constants
    const int WEBCAM_WIDTH = 640;
    const int WEBCAM_HEIGHT = 480;
    private string _stream;
    public SKElement _element;
    const int FPS = 30;
    #endregion

    public StreamRunner()
    {
        //_element = element;
        _sortTracker = new SortTracker(costThreshold: 0.5f, maxAge: 5, tailLength: 30);
        _yolo = new Yolo(new YoloOptions
        {
            OnnxModel = @"D:\Dev\YoloDotNet\Demo\WebcamDemo\yolo12m.onnx",
            ExecutionProvider = new CpuExecutionProvider(),
            ImageResize = ImageResize.Proportional,
            SamplingOptions = new(SKFilterMode.Nearest, SKMipmapMode.None)
        });
        _dispatcher = Dispatcher.CurrentDispatcher;
        _currentFrame = new SKBitmap(WEBCAM_WIDTH, WEBCAM_HEIGHT);
        _rect = new SKRect(0, 0, WEBCAM_WIDTH, bottom: WEBCAM_HEIGHT);
        _imageInfo = new SKImageInfo(WEBCAM_WIDTH, WEBCAM_HEIGHT, SKColorType.Bgra8888, SKAlphaType.Premul);
        //Task.Run(() => StreamAsync());
    }

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public async Task StreamAsync(string stream,SKElement element)
    {
        _element = element;
        _stream = stream;
        using var capture = new VideoCapture(_stream, VideoCapture.API.Ffmpeg);
        capture.Set(CapProp.Fps, FPS);
        capture.Set(CapProp.FrameWidth, WEBCAM_WIDTH);
        capture.Set(CapProp.FrameHeight, WEBCAM_HEIGHT);
        using var mat = new Mat();
        using var bgraMat = new Mat();
        using var resizeMat = new Mat();
        var sellerRoi = new ObjectDetection()
        {
            BoundingBox = new SKRectI { Location = new SKPointI() { X = 0, Y = 0, }, Size = new SKSizeI() { Height =100, Width = 700 } },
            Label = new LabelModel() { Name = "sellerRoi" },
        };
        var customerRegion = new ObjectDetection()
        {
            BoundingBox = new SKRectI { Location = new SKPointI() { X = 0, Y = 0 }, Size = new SKSizeI() { Height = 100, Width = 700 } },
            Label = new LabelModel() { Name = "customerRoi" },
        };
        var regions = new List<ObjectDetection>() { sellerRoi, customerRegion };
        var SellerLabel = new LabelModel { Name = "seller" };
        var CustomerLabel = new LabelModel { Name = "customer" };
        while (true)
        {
            // Capture the current frame from the webcam
            capture.Read(mat);
            CvInvoke.Resize(mat, resizeMat, new System.Drawing.Size(640, 480), 0, 0, Inter.Linear);
            CvInvoke.CvtColor(resizeMat, bgraMat, ColorConversion.Bgr2Bgra);
            //Create an SKBitmap from the BGRA Mat for processing
            using var frame = SKImage.FromPixels(_imageInfo, bgraMat.DataPointer, (int)bgraMat.Step);
            _currentFrame = SKBitmap.FromImage(frame);
            if (_runDetection)
            {
                //_stopwatch.Restart();
                // Run object detection on the current frame
                var results = _yolo.RunObjectDetection(_currentFrame, 0.5, iou: 0.7);
                if (results.Count() != 0)
                {
                   /*var findSeller = results.Where(r => r.Label.Name == "person").Where(p => p.BoundingBox.IntersectsWith(sellerRoi.BoundingBox)).ToList().Select(p => new ObjectDetection() { BoundingBox = p.BoundingBox, Confidence = p.Confidence, Id = p.Id, Label = SellerLabel, Tail = p.Tail }).First();
                    var index = results.FindIndex(i => i.Id == findSeller.Id);
                    if (index != -1)
                        results[index] = findSeller;
                    var findCustomers = results.Where(r => r.Label.Name == "person").Where(p => p.BoundingBox.IntersectsWith(customerRegion.BoundingBox)).ToList().Select(p => new ObjectDetection() { BoundingBox = p.BoundingBox, Confidence = p.Confidence, Id = p.Id, Label = SellerLabel, Tail = p.Tail }).ToList();
                    foreach (var customer in findCustomers)
                    {
                        var index1 = results.FindIndex(i => i.Id == customer.Id);
                        if (index1 != -1)
                            results[index1] = customer;
                    }*/
                    //results = results.Where(r => r.Label.Name == "person").Where(p => p.BoundingBox.IntersectsWith(region2.BoundingBox)).ToList().Select(p => new ObjectDetection(){ BoundingBox = p.BoundingBox, Confidence = p.Confidence, Id = p.Id, Label = CustomerLabel, Tail = p.Tail}).ToList();
                    //var seller = persons.Where(p => p.BoundingBox.IntersectsWith(region.BoundingBox));
                    
                }
                if (_isFilteringEnabled)
                    results = results.FilterLabels(["person"]);  // Optionally filter results to include only specific classes (e.g., "person", "cat", "dog")
                if (_isTrackingEnabled)
                    results.Track(_sortTracker); // Optionally track objects using the SortTracker
                // Draw detection and tracking results on the current frame
                //results.Add(region);
                //results.Add(region2);
                //_currentFrame.Draw(regions);
                _currentFrame.Draw(results);
                //_stopwatch.Stop();
            }
            // Update GUI
            await _dispatcher.InvokeAsync(() =>
            {
                _element.InvalidateVisual(); // Notify SKiaSharp to update the frame.
            });
        }
    }
    /*public void UpdateStreamFrame(object sender, SKPaintSurfaceEventArgs e)
    {
        using var canvas = e.Surface.Canvas;
        canvas.DrawBitmap(_currentFrame, _rect);
        canvas.Flush();
    }*/
    CommandHandler? updateStreamFrameCommand;
    public CommandHandler UpdateStreamFrameCommand
        {
        get
        {
            return updateStreamFrameCommand ??
                (updateStreamFrameCommand = new CommandHandler(obj =>
                {
                    SKPaintSurfaceEventArgs? e = obj as SKPaintSurfaceEventArgs;
                    if (e != null)
                    {
                        using var canvas = e.Surface.Canvas;
                        canvas.DrawBitmap(CurrentFrame, _rect);
                        canvas.Flush();
                    }
                }));
        }
    }
}