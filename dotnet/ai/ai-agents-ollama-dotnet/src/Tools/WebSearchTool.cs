namespace AIAgentsOllama.Tools;

/// <summary>
/// Mock tool for web search (returns simulated results).
/// In production, integrate with a real search API like Bing or Google.
/// </summary>
public class WebSearchTool : ITool
{
    public string Name => "web_search";
    public string Description => "Searches the web for information. Input: query (search string)";

    public Task<string> ExecuteAsync(Dictionary<string, object> parameters)
    {
        if (!parameters.TryGetValue("query", out var query) || query == null)
            return Task.FromResult("Error: 'query' parameter is required");

        // Mock search results
        var mockResults = new Dictionary<string, string>
        {
            { "USD to CAD", "[{\"title\":\"USD to CAD Exchange Rate\",\"snippet\":\"1 USD = 1.42 CAD (as of today)\"}]" },
            { "weather paris", "[{\"title\":\"Paris Weather\",\"snippet\":\"Clear skies, 22°C, Light breeze\"}]" },
            { "dotnet agents", "[{\"title\":\"Building AI Agents with .NET\",\"snippet\":\"Learn how to create autonomous agents using C# and Ollama\"}]" },
        };

        var queryStr = query.ToString()?.ToLower() ?? "";
        foreach (var (key, value) in mockResults)
        {
            if (queryStr.Contains(key.ToLower()))
                return Task.FromResult(value);
        }

        return Task.FromResult($"[{{\"title\":\"Search Results for '{query}'\",\"snippet\":\"No specific results found, but search was successful\"}}]");
    }
}
