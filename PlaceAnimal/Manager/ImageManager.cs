using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Caching.Memory;
using PlaceAnimal.Dto;
using PlaceAnimal.Enums;
using PlaceAnimal.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace PlaceAnimal.Manager;

public class ImageManager
{
    private readonly BlobStorageOptions _options;
    private readonly IMemoryCache _cache;

    public const string ContentType = "image/jpeg";

    public ImageManager(BlobStorageOptions options, IMemoryCache cache)
    {
        _options = options;
        _cache = cache;
    }
    
    public async Task<byte[]> GetImage(ImageTriggerQuery query)
    {
        BlobContainerClient client = await GetBlobContainerClient();

        if (_cache.TryGetValue(query.CacheKey, out byte[] cacheResult))
        {
            return cacheResult;
        }

        Pageable<BlobItem> blobs = client.GetBlobs(BlobTraits.None, BlobStates.None, query.StorageQueryString);
        
        List<BlobItem> items = blobs.AsPages().SelectMany(p => p.Values.Select(val => val)).ToList();
        
        string blobName = items[Random.Shared.Next(items.Count)].Name;

        BlobClient blobClient = client.GetBlobClient(blobName);

        using BlobDownloadInfo response = await blobClient.DownloadAsync();
        using Image image = await Image.LoadAsync(response.Content);
        using MemoryStream resized = new MemoryStream();

        image.Mutate(c => c.CropToAspectRatio(query.AspectRatio).Resize(query.Width, query.Height));
                
        await image.SaveAsync(resized, new JpegEncoder { Quality = 80 });

        byte[] bytes = resized.ToArray();

        _cache.Set(query.CacheKey, bytes);

        return bytes;
    }
    
    private async Task<BlobContainerClient> GetBlobContainerClient()
    {
        BlobServiceClient client = new BlobServiceClient(_options.ConnectionString);

        BlobContainerClient containerClient = client.GetBlobContainerClient(_options.ImageContainerName);

        await containerClient.CreateIfNotExistsAsync();

        return containerClient;
    }
}