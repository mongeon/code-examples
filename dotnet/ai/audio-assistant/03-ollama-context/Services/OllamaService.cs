using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace AudioAssistant.Services;

public interface ILlmService
{
    Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken);
}

public class OllamaService : ILlmService
{
    private readonly HttpClient _http;
    private readonly AssistantOptions _options;
    private readonly ILogger<OllamaService> _logger;

    public OllamaService(HttpClient http, IOptions<AssistantOptions> options, ILogger<OllamaService> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Envoi au LLM : \"{Prompt}\"", prompt);

        var request = new
        {
            model = _options.OllamaModel,
            prompt = prompt,
            stream = false
        };

        var response = await _http.PostAsJsonAsync(
            $"{_options.OllamaBaseUrl}/api/generate",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(
            cancellationToken: cancellationToken);

        var text = result?.Response?.Trim() ?? "Je n'ai pas de réponse.";
        _logger.LogInformation("Réponse du LLM : \"{Text}\"", text);
        return text;
    }
}

internal record OllamaResponse(string Response);