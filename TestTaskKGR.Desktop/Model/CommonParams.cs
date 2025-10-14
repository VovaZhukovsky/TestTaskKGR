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
}

public enum Role
{
    Seller,
    Customer
}

