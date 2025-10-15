using SkiaSharp;
using System.IO;
using YoloDotNet.Models;

namespace TestTaskKGR.Desktop.Model;

public class CommonParams
{
    public int StreamWidth = 640;
    public int StreamHeigth = 480;
    public int FPS = 30;
    public string OnnexMainModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Onnx", "yolo12m.onnx");
    public string OnnexBottleModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Onnx", "bottle_best.onnx");
    public string OnnexPhoneModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Onnx", "phone_best.onnx");

    public ObjectDetection SellerRoiStream1 = new ObjectDetection()
    {
        BoundingBox = new SKRectI { Location = new SKPointI() { X = 0, Y = 300, }, Size = new SKSizeI() { Height = 100, Width = 700 } },
        Label = new LabelModel() { Name = "sellerRoi" },
    };
    public ObjectDetection CustomerRoiStream1 = new ObjectDetection()
    {
        BoundingBox = new SKRectI { Location = new SKPointI() { X = 0, Y = 50 }, Size = new SKSizeI() { Height = 200, Width = 700 } },
        Label = new LabelModel() { Name = "customerRoi" },
    };
    public LabelModel SellerLabel = new LabelModel { Name = "seller" };
    public LabelModel CustomerLabel = new LabelModel { Name = "customer" };
}

public enum Role // подключить viewmodel class
{
    Seller,
    Customer
}

