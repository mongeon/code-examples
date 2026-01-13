namespace AIAgentsOllama.Tools;

/// <summary>
/// Interface for tools that agents can invoke.
/// </summary>
public interface ITool
{
    /// <summary>
    /// Name of the tool (used for invocation).
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Description of what the tool does (helps LLM decide when to use it).
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Executes the tool with the given parameters.
    /// </summary>
    /// <param name="parameters">Parameters for the tool</param>
    /// <returns>Result of tool execution as a string</returns>
    Task<string> ExecuteAsync(Dictionary<string, object> parameters);
}
