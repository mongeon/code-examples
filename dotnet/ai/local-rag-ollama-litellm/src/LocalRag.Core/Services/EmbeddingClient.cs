using LocalRag.Core.Models;
using System.Net.Http.Json;

namespace LocalRag.Core.Services;

public class EmbeddingClient(HttpClient httpClient)
{
    public async Task<float[]> EmbedAsync(string text, string model = "nomic-embed-text")
    {
        try
        {
            var request = new EmbedRequest(text, model);
            var response = await httpClient.PostAsJsonAsync("/v1/embeddings", request);
            response.EnsureSuccessStatusCode();

            var embedResponse = await response.Content.ReadFromJsonAsync<EmbedResponse>();
            return embedResponse?.Data?.FirstOrDefault()?.Embedding ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Embedding error: {ex.Message}");
            throw;
        }
    }
}