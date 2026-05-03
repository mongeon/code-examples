using Microsoft.Extensions.Options;

namespace AudioAssistant.Services;

public interface IContextService
{
    void Initialize();
    List<ConversationMessage> AddUserMessage(string userInput);
    void AddAssistantMessage(string response);
    void Reset();
}

public class ContextService : IContextService
{
    private readonly AssistantOptions _options;
    private readonly List<ConversationMessage> _history = new();
    private readonly ILogger<ContextService> _logger;

    public ContextService(IOptions<AssistantOptions> options, ILogger<ContextService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public void Initialize()
    {
        _history.Clear();
        _history.Add(new ConversationMessage("system", _options.SystemPrompt));
        _logger.LogInformation("Contexte initialisé.");
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