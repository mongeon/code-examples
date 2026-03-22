using Microsoft.Agents.AI;
using OllamaSharp;

namespace AgentFrameworkOllamaMcp.Examples;

/// <summary>
/// Example 1: Hello World — a single agent connected to Ollama.
/// No tools, no workflow. Just an agent that talks to a local LLM.
/// </summary>
public static class Example1_HelloAgent
{
    public static async Task RunAsync()
    {
        // OllamaApiClient implements IChatClient — Agent Framework accepts any IChatClient.
        var chatClient = new OllamaApiClient(
            new Uri("http://localhost:11434"), "llama3.2");

        // Create the agent: a name, instructions, and a chat client. That's it.
        AIAgent agent = new ChatClientAgent(
            chatClient,
            name: "Assistant",
            instructions: "You are a concise assistant. Answer in two sentences or fewer.");

        // RunAsync sends the prompt, handles the conversation loop, and returns the final response.
        var response = await agent.RunAsync("What is the Model Context Protocol?");

        Console.WriteLine(response);
    }
}
