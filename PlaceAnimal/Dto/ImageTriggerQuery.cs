using PlaceAnimal.Enums;

namespace PlaceAnimal.Dto;

public class ImageTriggerQuery
{
    public int Height { get; set; } = 267;
    public int Width { get; set; } = 400;
    public Animal Animal { get; set; } = Animal.Dog;

    public decimal AspectRatio => (decimal) Width / Height;
    
    public string StorageQueryString => $"__default_{Animal.ToString().ToLower()}";

    public string CacheKey => $"{Animal}__{Width}__{Height}";
}