using AIAgentsOllama.Models;
using AIAgentsOllama.Tools;
using AIAgentsOllama.Utilities;
using System.Text.Json;

namespace AIAgentsOllama.Agents;

/// <summary>
/// Core AI Agent implementation using Ollama and the ReAct pattern.
/// </summary>
public class OllamaAgent
{
    private readonly OllamaClient _ollama;
    private readonly string _model;
    private ToolRegistry? _tools;
    private readonly List<ChatMessage> _conversationHistory = new();

    public OllamaAgent(string model = "llama3.3", string ollamaUrl = "http://localhost:11434")
    {
        _model = model;
        _ollama = new OllamaClient(ollamaUrl);
    }

    /// <summary>
    /// Sets the available tools for this agent.
    /// </summary>
    public void SetTools(ToolRegistry tools)
    {
        _tools = tools;
    }

    /// <summary>
    /// Runs the agent with the given goal.
    /// </summary>
    public async Task<string> RunAsync(string goal, int maxIterations = 10)
    {
        _conversationHistory.Clear();
        _conversationHistory.Add(new ChatMessage("user", goal));

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            var systemPrompt = BuildSystemPrompt();
            var messages = new List<ChatMessage> { new ChatMessage("system", systemPrompt) };
            messages.AddRange(_conversationHistory);

            var response = await _ollama.ChatAsync(_model, messages);
            _conversationHistory.Add(new ChatMessage("assistant", response));

            // Check if this is a tool call or final answer
            var toolCall = ExtractToolCall(response);
            if (toolCall != null)
            {
                Console.WriteLine($"[Agent] Calling tool: {toolCall.Name}");
                var toolResult = await ExecuteToolAsync(toolCall);
                Console.WriteLine($"[Agent] Tool result: {toolResult}");
                _conversationHistory.Add(new ChatMessage("tool", toolResult));
            }
            else
            {
                // Final answer
                return response;
            }
        }

        throw new InvalidOperationException($"Agent failed to reach a conclusion within {maxIterations} iterations");
    }

    private string BuildSystemPrompt()
    {
        var toolDescriptions = _tools?.GetToolDescriptions() ?? "No tools available";
        return $@"You are a helpful AI assistant. You can use the following tools:

{toolDescriptions}

To use a tool, respond EXACTLY in this format:
TOOL: tool_name
PARAMETERS: {{""param_name"": ""param_value""}}

When you have a final answer, respond with just your answer, no tool prefix.

Think step by step and use tools when needed to solve the problem.";
    }

    private ToolCall? ExtractToolCall(string response)
    {
        if (!response.Contains("TOOL:"))
            return null;

        var lines = response.Split('\n');
        string? toolName = null;
        string? parametersJson = null;

        foreach (var line in lines)
        {
            if (line.StartsWith("TOOL:"))
            {
                toolName = line["TOOL:".Length..].Trim();
            }
            else if (line.StartsWith("PARAMETERS:"))
            {
                parametersJson = line["PARAMETERS:".Length..].Trim();
            }
        }

        if (toolName != null && parametersJson != null)
        {
            try
            {
                var parameters = JsonSerializer.Deserialize<Dictionary<string, object>>(parametersJson) ?? new();
                return new ToolCall(toolName, parameters);
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    private async Task<string> ExecuteToolAsync(ToolCall toolCall)
    {
        if (_tools == null)
            return "Error: No tools available";

        try
        {
            return await _tools.ExecuteToolAsync(toolCall.Name, toolCall.Parameters);
        }
        catch (Exception ex)
        {
            return $"Error executing tool: {ex.Message}";
        }
    }
}
