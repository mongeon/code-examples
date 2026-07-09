# MCP Books Server for Claude Code

A C# MCP (Model Context Protocol) server that wraps a small internal API — a personal reading tracker — and plugs into Claude Code.

Companion code for the blog post: [Build an MCP server in C# and plug it into Claude Code](https://www.gabrielmongeon.ca/en/blog/mcp-server-csharp-claude-code)

## Project Structure

```
mcp-books-claude-code/
  BooksApi/
    BooksApi.csproj    # Minimal API (net10.0), in-memory storage
    Program.cs         # GET /books, GET /books/search, POST /books on port 5200
  BooksMcp/
    BooksMcp.csproj    # Project file (net10.0, ModelContextProtocol SDK)
    Program.cs         # Host setup, MCP server registration, logs to stderr
    BooksClient.cs     # Typed HTTP client for the internal API
    BooksTools.cs      # MCP tool definitions (ListBooks, SearchBooks, AddBook)
  .mcp.json            # Sample project-scope config for Claude Code
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Claude Code](https://code.claude.com/) (for connecting the server)

## Running

Start the internal API first (it listens on `http://localhost:5200`):

```bash
dotnet run --project BooksApi
```

The MCP server itself communicates over stdin/stdout (JSON-RPC 2.0). It's meant to be launched by an MCP client, not run interactively.

## Connecting to Claude Code

From this folder, a single command is enough:

```bash
claude mcp add books -- dotnet run --project ./BooksMcp
```

Everything after the `--` is the command Claude Code launches as a child process, with stdin/stdout as the JSON-RPC channel. By default the config uses the `local` scope; use `--scope project` to write a `.mcp.json` at the repo root (a sample is included here), or `--scope user` to make the server available in all your projects.

Check the wiring with `/mcp` in Claude Code, then test with a real request: *"What's on my reading list right now?"* or *"Add Refactoring by Martin Fowler to my to-read list"*.

## Notes

- The MCP server contains no business logic: each tool calls the API and formats the response for the model.
- Logs are written to stderr. stdout is reserved for the JSON-RPC transport — any `Console.WriteLine` in a tool will break the protocol.
- If the server shows up as failed in `/mcp`, the problem is almost always the startup command: a relative project path pointing to the wrong place, or the internal API not running.
- The blog post used the `ModelContextProtocol` SDK while it was in preview; the package has since reached a stable release, pinned in the `.csproj`.
