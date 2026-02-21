using System.Text;
using System.Text.Json;
using AIAgentsOllama.Models;

namespace AIAgentsOllama.Utilities;

/// <summary>
/// HTTP client wrapper for Ollama API.
/// </summary>
public class OllamaClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public OllamaClient(string baseUrl = "http://localhost:11434")
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
    }

    /// <summary>
    /// Sends a chat message to Ollama and gets a response.
    /// </summary>
    public async Task<string> ChatAsync(string model, List<ChatMessage> messages)
    {
        var request = new
        {
            model,
            messages = messages.Select(m => new { role = m.Role, content = m.Content }),
            stream = false
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/chat", content);
            var responseText = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var jsonDoc = JsonDocument.Parse(responseText);
                return jsonDoc.RootElement.GetProperty("message").GetProperty("content").GetString() ?? "";
            }

            throw new InvalidOperationException($"Ollama error: {responseText}");
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Failed to connect to Ollama at {_baseUrl}. Make sure Ollama is running.", ex);
        }
    }
}
