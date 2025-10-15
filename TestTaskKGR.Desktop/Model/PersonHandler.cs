using OpenTK.Graphics.OpenGL;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoloDotNet.Models;
using static Emgu.CV.Dai.OpenVino;

namespace TestTaskKGR.Desktop.Model;

public class PersonHandler
{
    public static async Task FindPersonByRoleAsync(List<ObjectDetection> results, LabelModel label, SKRectI boundinxbox)
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
    public static async Task<List<ObjectDetection>> GetPersonRoiAsync(List<ObjectDetection> persons)
    {
        List<ObjectDetection> personsRoi = new();
        foreach(var person in persons)
        {
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
                Label = new LabelModel { Name = "seller_roi" },
                Confidence = person.Confidence,
                Id = person.Id
            });
        }
        return personsRoi;
    }
    public static async Task FindBottleAsync(List<ObjectDetection> personsRoi, List<ObjectDetection> results)
    {
        foreach (var roi in personsRoi)
        {
            if (results.Any(bottle => roi.BoundingBox.Contains(bottle.BoundingBox)))
            {
               // _logger.Log($"Нарушение {roi.Label.Name} пьет воду");
            }
        }
    }
    public static async Task FindPhoneAsync(List<ObjectDetection> sellerRoi, List<ObjectDetection> results)
    {
        foreach (var roi in sellerRoi)
        {
            if (results.Any(phone => roi.BoundingBox.Contains(phone.BoundingBox)))
            {
                
                // _logger.Log($"Нарушение {roi.Label.Name} пьет воду");
            }
        }
    }
}
