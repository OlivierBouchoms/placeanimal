using System;

namespace PlaceAnimal.Options;

public class BlobStorageOptions
{
    private static BlobStorageOptions _instance;
    public static BlobStorageOptions Instance => _instance ??= new BlobStorageOptions();

    public string ConnectionString => Environment.GetEnvironmentVariable("BLOB_CONNECTION");

    public string ImageContainerName => Environment.GetEnvironmentVariable("BLOB_CONTAINER");
}