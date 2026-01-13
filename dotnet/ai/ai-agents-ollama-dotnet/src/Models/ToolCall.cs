namespace AIAgentsOllama.Models;

/// <summary>
/// Represents a tool invocation made by the agent.
/// </summary>
public class ToolCall
{
    public ToolCall(string name, Dictionary<string, object> parameters)
    {
        Name = name;
        Parameters = parameters;
    }

    /// <summary>
    /// Name of the tool to invoke.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Parameters to pass to the tool.
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; }
}
