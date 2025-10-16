using SkiaSharp;
using System.IO;
using YoloDotNet.Models;

namespace TestTaskKGR.Desktop.Model;

public class CommonParams
{
    public int StreamWidth = 800;
    public int StreamHeigth = 600;
    public int FPS = 30;
    public string OnnexMainModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Onnx", "yolo12m.onnx");
    public string OnnexBottleModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Onnx", "bottle_best.onnx");
    public string OnnexPhoneModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Onnx", "phone_best.onnx");

    public ObjectDetection SellerRoiStream1 = new ObjectDetection()
    {
        BoundingBox = new SKRectI { Location = new SKPointI() { X = 0, Y = 300, }, Size = new SKSizeI() { Height = 200, Width = 800 } },
        Label = new LabelModel() { Name = "sellerSpace" },
    };
    public ObjectDetection CustomerRoiStream1 = new ObjectDetection()
    {
        BoundingBox = new SKRectI { Location = new SKPointI() { X = 0, Y = 50 }, Size = new SKSizeI() { Height = 300, Width = 800 } },
        Label = new LabelModel() { Name = "customerSpace" },
    };
    public LabelModel SellerLabel = new LabelModel { Name = "seller" };
    public LabelModel CustomerLabel = new LabelModel { Name = "customer" };
}

