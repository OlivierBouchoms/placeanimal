using System;

namespace PlaceAnimal.Options;

public class BlobStorageOptions
{
    private static BlobStorageOptions _instance;
    public static BlobStorageOptions Instance => _instance ??= new BlobStorageOptions();
    
    public string ConnectionString => Environment.GetEnvironmentVariable("BLOB_CONNECTION") ??
                                       "DefaultEndpointsProtocol=https;AccountName=placeanimal8bbb;AccountKey=+kDD8httQRd1Qx9ASFnRdHF5Ya3zpfkm1Uk13gOnyI95mox/Hawb3zQYrCpEeC8CtVpKTDHu8ZIV+AStQ4gTzQ==;EndpointSuffix=core.windows.net";

    public string ImageContainerName => Environment.GetEnvironmentVariable("BLOB_CONTAINER") ?? "placeanimal";
}