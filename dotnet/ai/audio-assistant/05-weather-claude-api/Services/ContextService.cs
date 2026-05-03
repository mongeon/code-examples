using Microsoft.Extensions.Options;

namespace AudioAssistant.Services;

public interface IContextService
{
    Task InitializeAsync(CancellationToken cancellationToken);
    List<ConversationMessage> AddUserMessage(string userInput);
    void AddAssistantMessage(string response);
    void Reset();
}

public class ContextService : IContextService
{
    private readonly AssistantOptions _options;
    private readonly IWeatherService _weather;
    private readonly List<ConversationMessage> _history = new();
    private readonly ILogger<ContextService> _logger;

    public ContextService(
        IOptions<AssistantOptions> options,
        IWeatherService weather,
        ILogger<ContextService> logger)
    {
        _options = options.Value;
        _weather = weather;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var meteo = await _weather.GetCurrentWeatherAsync(cancellationToken);
        var systemPromptAvecMeteo = _options.SystemPrompt +
            $"\n\nMétéo actuelle à Blainville : {meteo}";

        _history.Clear();
        _history.Add(new ConversationMessage("system", systemPromptAvecMeteo));
        _logger.LogInformation("Contexte initialisé avec météo : {Meteo}", meteo);
    }

    public List<ConversationMessage> AddUserMessage(string userInput)
    {
        _history.Add(new ConversationMessage("user", userInput));
        _logger.LogInformation("Historique : {Count} messages", _history.Count);
        return _history;
    }

    public void AddAssistantMessage(string response)
    {
        _history.Add(new ConversationMessage("assistant", response));
    }

    public void Reset()
    {
        _history.Clear();
        _logger.LogInformation("Historique réinitialisé.");
    }
}