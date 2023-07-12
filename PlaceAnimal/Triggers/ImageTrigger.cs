using System;
using System.Threading.Tasks;
using System.Web.Http;
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
        if (string.IsNullOrEmpty(animal))
        {
            animal = Animal.Dog.ToString();
        }

        var isAnimalValid = Enum.TryParse<Animal>(animal, true, out var parsedAnimal);

        if (!isAnimalValid)
        {
            return new BadRequestObjectResult(new[]
            {
                new { Property = nameof (animal), Message = $"Valid values are {string.Join(", ", Enum.GetNames<Animal>())}." }
            });
        }

        ImageTriggerQuery query = new ImageTriggerQuery
        {
            Width = Math.Clamp(width.GetValueOrDefault(AppConfig.DefaultWidth), 1, AppConfig.MaxWidth),
            // Use height as first fallback, then default height
            Height = Math.Clamp(height.GetValueOrDefault(width.GetValueOrDefault(AppConfig.DefaultHeight)), 1, AppConfig.MaxHeight),
            Animal = parsedAnimal
        };

        try
        {
            byte[] image = await ImageManager.GetImage(query);

            return new FileContentResult(image, ImageManager.ContentType);
        } catch (Exception e)
        {
            return new ObjectResult(new { Error = "An internal server error occurred." }) { StatusCode = 500 };
        }
    }
}