namespace AIAgentsOllama.Models;

/// <summary>
/// Response from the agent, which can be a final answer or a tool call.
/// </summary>
public class AgentResponse
{
    public required string Content { get; set; }
    public ToolCall? ToolCall { get; set; }
    public bool IsFinalAnswer { get; set; }
}
