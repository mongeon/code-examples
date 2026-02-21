# MCP Weather Server

A minimal MCP (Model Context Protocol) server in C# that exposes a weather tool using the [Open-Meteo](https://open-meteo.com/) public API — no key required.

Companion code for the blog post: [MCP in C#: Exposing Your Own Tools to Any AI Client](https://www.gabrielmongeon.ca/en/blog/mcp-csharp-weather-server)

## Project Structure

```
mcp-weather-server/
  mcp-weather-server.csproj   # Project file (net9.0, ModelContextProtocol SDK)
  Program.cs                  # Host setup, MCP server registration
  WeatherService.cs           # Open-Meteo API client (geocoding + weather)
  WeatherTools.cs             # MCP tool definition ([McpServerTool])
  GeocodingResponse.cs        # Deserialization model for geocoding API
  OpenMeteoResponse.cs        # Deserialization model for weather API
  WeatherResult.cs            # Internal result record
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- Claude Desktop or VS Code with GitHub Copilot (for connecting the server)

## Running

```bash
dotnet run --project mcp-weather-server
```

The server communicates over stdin/stdout (JSON-RPC 2.0). It's meant to be launched by an MCP client, not run interactively.

## Connecting to Claude Desktop

Add to `%APPDATA%\Claude\claude_desktop_config.json` (Windows) or `~/Library/Application Support/Claude/claude_desktop_config.json` (macOS):

```json
{
  "mcpServers": {
    "weather": {
      "command": "dotnet",
      "args": ["run", "--project", "C:\\path\\to\\mcp-weather-server"]
    }
  }
}
```

Restart Claude Desktop. Ask: *"What's the weather in Montreal?"*

## Connecting to VS Code Copilot

Add `.vscode/mcp.json` at the workspace root:

```json
{
  "servers": {
    "weather": {
      "type": "stdio",
      "command": "dotnet",
      "args": ["run", "--project", "${workspaceFolder}/mcp-weather-server"]
    }
  }
}
```

Enable Agent mode in Copilot Chat.

## Notes

- The `ModelContextProtocol` SDK is in preview. The version is pinned in the `.csproj`.
- Logs are written to stderr. stdout is reserved for the JSON-RPC transport — any `Console.WriteLine` in a tool will break the protocol.
