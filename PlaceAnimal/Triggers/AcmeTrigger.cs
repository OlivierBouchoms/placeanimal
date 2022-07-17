using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using PlaceAnimal.Manager;
using PlaceAnimal.Options;

namespace PlaceAnimal;

public class AcmeTrigger
{
    private static SslManager _sslManager = new(BlobStorageOptions.Instance);
    
    [FunctionName("Acme")]
    public static async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ".well-known/acme-challenge/{challenge}")] 
        HttpRequest request, string challenge)
    {
        string result = await _sslManager.GetChallengeResult(request.Host.Host, challenge);
        
        return new ContentResult
        {
           Content = result,
           ContentType = "text/plain",
           StatusCode = !string.IsNullOrWhiteSpace(result) ? 200 : 404
        };
    }
}