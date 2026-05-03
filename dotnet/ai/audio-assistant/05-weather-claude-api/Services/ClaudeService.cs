namespace AudioAssistant.Services;

public class ClaudeService : ILlmService
{
    private readonly AssistantOptions _options;
    private readonly ILogger<ClaudeService> _logger;

    public ClaudeService(IOptions<AssistantOptions> options, ILogger<ClaudeService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> ChatAsync(List<ConversationMessage> history, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Envoi à Claude ({Count} messages)...", history.Count);

        var client = new AnthropicClient(_options.ClaudeApiKey);

        var systemPrompt = history
            .FirstOrDefault(m => m.Role == "system")?.Content ?? "";

        var messages = history
            .Where(m => m.Role != "system")
            .Select(m => new Message
            {
                Role = m.Role == "user" ? RoleType.User : RoleType.Assistant,
                Content = new List<ContentBase> { new TextContent { Text = m.Content } }
            })
            .ToList();

        var request = new MessageParameters
        {
            Model = _options.ClaudeModel,
            MaxTokens = 512,
            System = new List<SystemMessage> { new SystemMessage(systemPrompt) },
            Messages = messages
        };

        var response = await client.Messages.GetClaudeMessageAsync(request, cancellationToken);
        var text = response.Content.OfType<TextContent>().FirstOrDefault()?.Text?.Trim()
            ?? "Je n'ai pas de réponse.";

        _logger.LogInformation("Réponse de Claude : \"{Text}\"", text);
        return text;
    }
}