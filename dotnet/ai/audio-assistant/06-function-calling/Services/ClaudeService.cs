using CommonTool = Anthropic.SDK.Common.Tool;


namespace AudioAssistant.Services;

public class ClaudeService : ILlmService
{
    private readonly AssistantOptions _options;
    private readonly ToolRegistry _toolRegistry;
    private readonly ILogger<ClaudeService> _logger;

    public ClaudeService(
        IOptions<AssistantOptions> options,
        ToolRegistry toolRegistry,
        ILogger<ClaudeService> logger)
    {
        _options = options.Value;
        _toolRegistry = toolRegistry;
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

        // Construire les tools via Function wrapper
        var tools = _toolRegistry.GetAll()
            .Select(t => new Function(t.Name, t.Description, t.GetParametersSchema()))
            .Select(f => (CommonTool)f)
            .ToList();

        var request = new MessageParameters
        {
            Model = _options.ClaudeModel,
            MaxTokens = 512,
            System = new List<SystemMessage> { new SystemMessage(systemPrompt) },
            Messages = messages,
            Tools = tools
        };

        var response = await client.Messages.GetClaudeMessageAsync(request, cancellationToken);

        // Vérifier si Claude veut appeler un outil
        if (response.StopReason == "tool_use")
        {
            var toolUseBlock = response.Content.OfType<ToolUseContent>().FirstOrDefault();
            if (toolUseBlock != null)
            {
                _logger.LogInformation("Claude appelle l'outil : {Tool}", toolUseBlock.Name);

                var tool = _toolRegistry.GetByName(toolUseBlock.Name);
                if (tool != null)
                {
                    var toolResult = await tool.ExecuteAsync(
                        toolUseBlock.Input?.ToString() ?? "",
                        cancellationToken);

                    _logger.LogInformation("Résultat de l'outil : {Result}", toolResult);

                    messages.Add(new Message
                    {
                        Role = RoleType.Assistant,
                        Content = response.Content
                    });

                    messages.Add(new Message
                    {
                        Role = RoleType.User,
                        Content = new List<ContentBase>
                        {
                            new ToolResultContent
                            {
                                ToolUseId = toolUseBlock.Id,
                                Content = new List<ContentBase>
                                {
                                    new TextContent { Text = toolResult }
                                }
                            }
                        }
                    });

                    var finalResponse = await client.Messages.GetClaudeMessageAsync(
                        new MessageParameters
                        {
                            Model = _options.ClaudeModel,
                            MaxTokens = 512,
                            System = new List<SystemMessage> { new SystemMessage(systemPrompt) },
                            Messages = messages,
                            Tools = tools
                        }, cancellationToken);

                    var finalText = finalResponse.Content.OfType<TextContent>()
                        .FirstOrDefault()?.Text?.Trim() ?? "Je n'ai pas de réponse.";

                    _logger.LogInformation("Réponse finale de Claude : \"{Text}\"", finalText);
                    return finalText;
                }
            }
        }

        var text = response.Content.OfType<TextContent>()
            .FirstOrDefault()?.Text?.Trim() ?? "Je n'ai pas de réponse.";

        _logger.LogInformation("Réponse de Claude : \"{Text}\"", text);
        return text;
    }
}