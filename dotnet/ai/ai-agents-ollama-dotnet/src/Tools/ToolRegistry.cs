namespace AIAgentsOllama.Tools;

/// <summary>
/// Manages the collection of available tools for an agent.
/// </summary>
public class ToolRegistry
{
    private readonly Dictionary<string, ITool> _tools = new();

    /// <summary>
    /// Registers a new tool.
    /// </summary>
    public void Register(ITool tool)
    {
        _tools[tool.Name] = tool;
    }

    /// <summary>
    /// Gets a tool by name.
    /// </summary>
    public ITool? GetTool(string name)
    {
        return _tools.TryGetValue(name, out var tool) ? tool : null;
    }

    /// <summary>
    /// Gets all available tools.
    /// </summary>
    public IEnumerable<ITool> GetAllTools() => _tools.Values;

    /// <summary>
    /// Generates a formatted description of all tools.
    /// </summary>
    public string GetToolDescriptions()
    {
        return string.Join("\n", _tools.Values.Select(t => $"- {t.Name}: {t.Description}"));
    }

    /// <summary>
    /// Executes a tool by name.
    /// </summary>
    public async Task<string> ExecuteToolAsync(string toolName, Dictionary<string, object> parameters)
    {
        var tool = GetTool(toolName);
        if (tool == null)
            throw new InvalidOperationException($"Tool '{toolName}' not found");

        return await tool.ExecuteAsync(parameters);
    }
}
