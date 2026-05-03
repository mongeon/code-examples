using Anthropic.SDK.Messaging;
using System.Text.Json.Nodes;

namespace AudioAssistant.Services.Tools;

public class TimeTool : ITool
{
    public string Name => "get_current_time";

    public string Description =>
        "Retourne la date et l'heure actuelle. Appelle cet outil quand l'utilisateur " +
        "demande l'heure, la date, le jour de la semaine, ou le mois.";

    public InputSchema InputSchema => new InputSchema
    {
        Type = "object",
        Properties = new Dictionary<string, Property>(),
        Required = Array.Empty<string>()
    };

    public JsonObject GetParametersSchema() => new JsonObject
    {
        ["type"] = "object",
        ["properties"] = new JsonObject(),
        ["required"] = new JsonArray()
    };

    public Task<string> ExecuteAsync(string input, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var result = $"{now:dddd d MMMM yyyy}, {now:HH:mm}";
        return Task.FromResult(result);
    }
}