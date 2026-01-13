using AIAgentsOllama.Tools;

namespace AIAgentsOllama.Tests;

/// <summary>
/// Mock tools for testing agents without external dependencies.
/// </summary>
public class MockCalculatorTool : ITool
{
    public string Name => "calculator";
    public string Description => "Mock calculator for testing";

    public Task<string> ExecuteAsync(Dictionary<string, object> parameters)
    {
        if (!parameters.TryGetValue("expression", out var expr))
            return Task.FromResult("Error: expression required");

        // Mock specific calculations
        var expression = expr?.ToString() ?? "";
        if (expression.Contains("15") && expression.Contains("2450"))
            return Task.FromResult("367.5");

        return Task.FromResult("42");
    }
}

public class MockWebSearchTool : ITool
{
    public string Name => "web_search";
    public string Description => "Mock web search for testing";

    public Task<string> ExecuteAsync(Dictionary<string, object> parameters)
    {
        if (!parameters.TryGetValue("query", out var query))
            return Task.FromResult("Error: query required");

        var q = query?.ToString() ?? "";
        if (q.Contains("USD") || q.Contains("CAD"))
            return Task.FromResult("[{\"title\":\"Exchange Rate\",\"snippet\":\"1 USD = 1.42 CAD\"}]");

        return Task.FromResult("[{\"title\":\"Result\",\"snippet\":\"Sample result\"}]");
    }
}
