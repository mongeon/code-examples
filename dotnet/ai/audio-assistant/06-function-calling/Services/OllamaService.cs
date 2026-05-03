using System.Net.Http.Json;

namespace AudioAssistant.Services;

public interface ILlmService
{
    Task<string> ChatAsync(List<ConversationMessage> history, CancellationToken cancellationToken);
}

public record ConversationMessage(string Role, string Content);

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

    public async Task<string> ChatAsync(List<ConversationMessage> history, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Envoi au LLM ({Count} messages)...", history.Count);

        var request = new
        {
            model = _options.OllamaModel,
            messages = history.Select(m => new { role = m.Role, content = m.Content }),
            stream = false
        };

        var response = await _http.PostAsJsonAsync(
            $"{_options.OllamaBaseUrl}/api/chat",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(
            cancellationToken: cancellationToken);

        var text = result?.Message?.Content?.Trim() ?? "Je n'ai pas de réponse.";
        _logger.LogInformation("Réponse du LLM : \"{Text}\"", text);
        return text;
    }
}

internal record OllamaChatMessage(string Role, string Content);
internal record OllamaChatResponse(OllamaChatMessage Message);