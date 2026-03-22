# Microsoft Agent Framework with Ollama and MCP

Three progressive examples showing how to use [Microsoft Agent Framework](https://github.com/microsoft/agent-framework) with a local Ollama model and the MCP weather server from the previous blog post.

## Project Structure

```
agent-framework-ollama-mcp/
  AgentFrameworkOllamaMcp.csproj
  Program.cs
  examples/
    Example1_HelloAgent.cs        # Basic agent + Ollama
    Example2_McpWeather.cs         # Agent + MCP weather tools
    Example3_SequentialWorkflow.cs  # Two-agent sequential workflow
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Ollama](https://ollama.ai/) running locally with the `llama3.2` model pulled
- The [mcp-weather-server](../../mcp/mcp-weather-server/) project (for examples 2 and 3)

### Pull the model

```bash
ollama pull llama3.2
```

## Running the Examples

```bash
# Example 1: Hello World agent (Ollama only, no tools)
dotnet run -- 1

# Example 2: Agent + MCP weather server
dotnet run -- 2

# Example 3: Sequential workflow with 2 agents
dotnet run -- 3
```

### Example 1 — Hello World Agent

A minimal agent: an `OllamaApiClient` (which implements `IChatClient`), a name, instructions, and `RunAsync`. No tools, no plumbing.

### Example 2 — Agent + MCP Weather

Connects to the MCP weather server via stdio transport. Agent Framework discovers the `GetCurrentWeather` tool through MCP and calls it automatically when the user asks about weather. No manual tool parsing — the framework handles the ReAct loop.

### Example 3 — Sequential Workflow

Two agents chained together:
1. **WeatherFetcher** — has MCP tools, fetches raw weather data
2. **FriendlyFormatter** — no tools, rewrites the data as a casual French message with a clothing suggestion

Uses `AgentWorkflowBuilder.BuildSequential()` and streams the output.

## Architecture

```
User Question
    │
    ▼
┌───────────────────┐     MCP (stdio)     ┌───────────────────┐
│  WeatherFetcher   │ ──────────────────► │  mcp-weather-     │
│  (ChatClientAgent)│ ◄────────────────── │  server            │
│  + MCP tools      │                     │  (Open-Meteo API)  │
└───────────────────┘                     └───────────────────┘
    │
    │ Sequential workflow
    ▼
┌───────────────────┐
│ FriendlyFormatter │
│ (ChatClientAgent) │
│ No tools          │
└───────────────────┘
    │
    ▼
  Output
```

## Troubleshooting

- **Ollama not running**: Make sure Ollama is started (`ollama serve` or check the system tray on Windows).
- **Model not found**: Run `ollama pull llama3.2` first.
- **MCP server path**: Examples 2 and 3 expect the weather server at `../../../mcp/mcp-weather-server` relative to this project. Adjust the path in the code if your layout differs.
- **Tool calling not working**: Some smaller models have limited function-calling support. Try `qwen2.5` or `llama3.3` if `llama3.2` struggles with tool calls.

## Related Blog Post

Companion code for the blog post: [Microsoft Agent Framework: First Agent, MCP, and Multi-Agent Workflows](https://www.gabrielmongeon.ca/en/blog/microsoft-agent-framework-ollama-mcp)

## License

See the root [LICENSE](../../../LICENSE) file.
