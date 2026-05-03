using Anthropic.SDK.Messaging;
using System.Text.Json.Nodes;

namespace AudioAssistant.Services.Tools;

public interface ITool
{
    string Name { get; }
    string Description { get; }
    InputSchema InputSchema { get; }
    JsonObject GetParametersSchema();
    Task<string> ExecuteAsync(string input, CancellationToken cancellationToken);
}