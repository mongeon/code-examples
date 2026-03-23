using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using OllamaSharp;

namespace AgentFrameworkOllamaMcp.Examples;

/// <summary>
/// Example 2: An agent connected to the MCP weather server from the previous blog post.
/// Agent Framework handles the tool-calling loop automatically, no manual ReAct parsing.
/// </summary>
public static class Example2_McpWeather
{
    public static async Task RunAsync()
    {
        // 1. Start the MCP weather server as a child process via stdio transport.
        //    Adjust the path to match where you cloned the mcp-weather-server project.
        await using var mcpClient = await McpClientFactory.CreateAsync(
            new McpClientOptions { ClientInfo = new() { Name = "WeatherAgent", Version = "1.0.0" } },
            new StdioClientTransport(new StdioClientTransportOptions
            {
                Command = "dotnet",
                Arguments = ["run", "--project", "../../../mcp/mcp-weather-server"]
            }));

        // 2. Discover the tools exposed by the MCP server.
        var mcpTools = await mcpClient.ListToolsAsync();

        // 3. Build the Ollama chat client with function invocation support.
        //    UseFunctionInvocation() lets Agent Framework call tools automatically.
        IChatClient chatClient = new ChatClientBuilder(
                new OllamaApiClient(new Uri("http://localhost:11434"), "qwen3"))
            .UseFunctionInvocation()
            .Build();

        // 4. Create the agent with the MCP tools attached.
        AIAgent agent = new ChatClientAgent(
            chatClient,
            name: "WeatherAgent",
            instructions: "You help users check the weather. Use the available tools to get real data.",
            tools: [.. mcpTools]);

        // 5. Ask a question — the agent calls GetCurrentWeather via MCP automatically.
        var response = await agent.RunAsync("What is the weather in Montreal?");

        Console.WriteLine(response);
    }
}
