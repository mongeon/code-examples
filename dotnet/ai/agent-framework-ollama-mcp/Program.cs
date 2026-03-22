using AgentFrameworkOllamaMcp.Examples;

if (args.Length == 0)
{
    Console.WriteLine("Usage: dotnet run -- <example>");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  1  Hello World agent (Ollama only)");
    Console.WriteLine("  2  Agent + MCP weather server");
    Console.WriteLine("  3  Sequential workflow (2 agents)");
    return;
}

switch (args[0])
{
    case "1":
        await Example1_HelloAgent.RunAsync();
        break;
    case "2":
        await Example2_McpWeather.RunAsync();
        break;
    case "3":
        await Example3_SequentialWorkflow.RunAsync();
        break;
    default:
        Console.WriteLine($"Unknown example: {args[0]}");
        break;
}
