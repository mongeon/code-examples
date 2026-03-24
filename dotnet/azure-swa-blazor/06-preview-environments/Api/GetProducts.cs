using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Api;

public class GetProducts
{
    [Function("GetProducts")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequestData req)
    {
        // Simulated data, in real life you'd fetch this from a database
        var products = new[]
        {
            new { Id = 1, Name = "Widget", Price = 9.99 },
            new { Id = 2, Name = "Gadget", Price = 24.99 },
            new { Id = 3, Name = "Thingamajig", Price = 14.99 }
        };

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(products);
        return response;
    }
}
