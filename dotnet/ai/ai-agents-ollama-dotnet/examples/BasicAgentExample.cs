using AIAgentsOllama.Agents;
using AIAgentsOllama.Tools;

namespace AIAgentsOllama.Examples;

/// <summary>
/// Example: Basic agent with calculator and web search tools.
/// </summary>
public class BasicAgentExample
{
    public static async Task Main()
    {
        Console.WriteLine("=== AI Agent Example ===");
        Console.WriteLine("Ensure Ollama is running: ollama serve");
        Console.WriteLine("Pull a model: ollama pull llama3.2\n");

        try
        {
            // Create agent
            var agent = new OllamaAgent("llama3.2");

            // Register tools
            var toolRegistry = new ToolRegistry();
            toolRegistry.Register(new CalculatorTool());
            toolRegistry.Register(new WebSearchTool());
            agent.SetTools(toolRegistry);

            // Run agent
            var goal = "What is 15% of 2,450? Then search for the current USD to CAD exchange rate.";
            Console.WriteLine($"Goal: {goal}\n");

            var result = await agent.RunAsync(goal, maxIterations: 10);
            Console.WriteLine($"\nAgent's Final Answer:\n{result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
