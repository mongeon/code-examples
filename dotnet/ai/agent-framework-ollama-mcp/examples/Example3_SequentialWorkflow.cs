using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using OllamaSharp;

namespace AgentFrameworkOllamaMcp.Examples;

/// <summary>
/// Example 3: A sequential workflow with two agents.
/// Agent 1 fetches weather data via MCP.
/// Agent 2 reformulates the raw data into a friendly, conversational response.
/// Each agent has a clear, testable responsibility.
/// </summary>
public static class Example3_SequentialWorkflow
{
    public static async Task RunAsync()
    {
        // 1. Start the MCP weather server.
        await using var mcpClient = await McpClientFactory.CreateAsync(
            new McpClientOptions { ClientInfo = new() { Name = "WorkflowDemo", Version = "1.0.0" } },
            new StdioClientTransport(new StdioClientTransportOptions
            {
                Command = "dotnet",
                Arguments = ["run", "--project", "../../../mcp/mcp-weather-server"]
            }));

        var mcpTools = await mcpClient.ListToolsAsync();

        // 2. Shared Ollama client — both agents use the same local model.
        var ollamaEndpoint = new Uri("http://localhost:11434");
        const string model = "qwen3";

        // 3. Agent 1: Weather Fetcher — has access to MCP tools.
        IChatClient fetcherClient = new ChatClientBuilder(
                new OllamaApiClient(ollamaEndpoint, model))
            .UseFunctionInvocation()
            .Build();

        AIAgent weatherFetcher = new ChatClientAgent(
            fetcherClient,
            name: "WeatherFetcher",
            instructions: """
                You are a weather data agent. When asked about weather,
                use your tools to get the current conditions.
                Return the raw weather data as-is, without commentary.
                """,
            tools: [.. mcpTools]);

        // 4. Agent 2: Friendly Formatter — no tools, just language skills.
        IChatClient formatterClient = new OllamaApiClient(ollamaEndpoint, model);

        AIAgent friendlyFormatter = new ChatClientAgent(
            formatterClient,
            name: "FriendlyFormatter",
            instructions: """
                You receive raw weather data. Rewrite it as a friendly,
                conversational message in French. Use casual language.
                Add a short clothing suggestion based on the conditions.
                """);

        // 5. Build a sequential workflow: Fetcher → Formatter.
        var workflow = AgentWorkflowBuilder.BuildSequential(weatherFetcher, friendlyFormatter);

        // 6. Run the workflow.
        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, "What is the weather in Montreal?")
        };

        var run = InProcessExecution.StreamAsync(workflow, messages);

        // 7. Stream the output events.
        await foreach (var ev in run.WatchStreamAsync())
        {
            if (ev is AgentResponseItem { Message: { } msg })
            {
                Console.WriteLine($"[{msg.AuthorName}] {msg.Text}");
            }
        }
    }
}
