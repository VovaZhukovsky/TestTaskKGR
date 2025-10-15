using OpenTK.Graphics.OpenGL;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoloDotNet.Models;
using TestTaskKGR.Desktop.Interfaces;
using TestTaskKGR.ApiClient;

namespace TestTaskKGR.Desktop.Model;

public class PersonHandler
{
    private ILogger _logger;
    private TestTaskKGRApiClient _httpClient;
    public PersonHandler(ILogger logger,TestTaskKGRApiClient httpClient)
    {
        _httpClient = httpClient;
        _logger = logger;
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
            switch(person.Label.Name)
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
