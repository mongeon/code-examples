using Microsoft.Extensions.Options;

namespace AudioAssistant.Services;

public interface IContextService
{
    string BuildPrompt(string userInput);
}

public class ContextService : IContextService
{
    private readonly AssistantOptions _options;

    public ContextService(IOptions<AssistantOptions> options)
    {
        _options = options.Value;
    }

    public string BuildPrompt(string userInput)
    {
        return _options.SystemPrompt
            + "\n\nQuestion de l'utilisateur : " + userInput
            + "\n\nRéponds en français, de façon concise (2-3 phrases maximum). Pas de mise en forme markdown.";
    }
}