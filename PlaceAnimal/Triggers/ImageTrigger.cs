using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using PlaceAnimal.Config;
using PlaceAnimal.Dto;
using PlaceAnimal.Enums;
using PlaceAnimal.Factory;
using PlaceAnimal.Manager;
using PlaceAnimal.Options;

namespace PlaceAnimal;

public static class ImageTrigger
{
    private static readonly ImageManager ImageManager = new(BlobStorageOptions.Instance, IMemoryCacheFactory.Instance);
    
    [FunctionName("ImageTrigger")]
    public static async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "/{width}/{height?}/{animal?}")] 
        HttpRequest request, int? width, int? height, string animal)
    {
        ImageTriggerQuery query = new ImageTriggerQuery
        {
            Width = Math.Clamp(width.GetValueOrDefault(AppConfig.DefaultWidth), 1, AppConfig.MaxWidth),
            // Use height as first fallback, then default height
            Height = Math.Clamp(height.GetValueOrDefault(width.GetValueOrDefault(AppConfig.DefaultHeight)), 1, AppConfig.MaxHeight),
            Animal = Enum.TryParse<Animal>(animal, true, out var parsedAnimal) ? parsedAnimal : AppConfig.DefaultAnimal
        };

        byte[] image = await ImageManager.GetImage(query);

        return new FileContentResult(image, ImageManager.ContentType);
    }
}