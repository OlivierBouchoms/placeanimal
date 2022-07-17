using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using PlaceAnimal.Options;

namespace PlaceAnimal.Manager;

public class SslManager
{
    private readonly BlobStorageOptions _blobStorageOptions;
    
    public SslManager(BlobStorageOptions blobStorageOptions)
    {
        _blobStorageOptions = blobStorageOptions;
    }
    
    public async Task<string> GetChallengeResult(string host, string challenge)
    {
        BlobContainerClient client = await GetBlobContainerClient(host);
        
        BlobClient blobClient = client.GetBlobClient(challenge);

        if (!blobClient.Exists())
        {
            return null;
        }
        
        using BlobDownloadInfo response = await blobClient.DownloadAsync();
        using StreamReader reader = new StreamReader(response.Content, Encoding.UTF8);

        return await reader.ReadToEndAsync();
    }
    
    private async Task<BlobContainerClient> GetBlobContainerClient(string host)
    {
        BlobServiceClient client = new BlobServiceClient(_blobStorageOptions.ConnectionString);

        BlobContainerClient containerClient = client.GetBlobContainerClient(GetContainerName(host));

        await containerClient.CreateIfNotExistsAsync();

        return containerClient;
    }

    private string GetContainerName(string host)
    {
        return host.Replace('.', '-');
    }

}