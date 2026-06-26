# Orchestrate Blazor and Ollama with .NET Aspire

A minimal Blazor chat app wired to a local [Ollama](https://ollama.ai/) model, with [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/) orchestrating everything: Ollama runs as a container, the model is a managed resource pulled at startup, and the Blazor app talks to it through `IChatClient` — no hard-coded URL, no manual `ollama pull`.

## Project Structure

```
aspire-blazor-ollama/
  OllamaBlazorAspire.slnx
  AppHost/
    AppHost.cs                 # Ollama + llama3.2 as resources; wires & waits for the web app
  ServiceDefaults/
    Extensions.cs              # OpenTelemetry, health checks, service discovery
  Web/                         # Blazor Web App (Interactive Server)
    Program.cs                 # IChatClient registration + Microsoft.Extensions.AI pipeline
    Components/
      Pages/
        Chat.razor             # Chat UI, streams responses via IChatClient
        Chat.razor.css
      Layout/
        NavMenu.razor
        MainLayout.razor
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) — Ollama runs in a container managed by Aspire
- (Optional) the Aspire CLI, so `aspire run` works:

  ```bash
  dotnet tool install -g aspire.cli
  ```

You do **not** need to install Ollama or pull a model by hand. Aspire pulls `llama3.2` into the container the first time you run.

## Running

From the solution root:

```bash
dotnet run --project AppHost
# or, with the Aspire CLI:
aspire run
```

The Aspire dashboard opens. Watch the `ollama` container start and the `chat` model download (first run pulls a few GB — give it a couple of minutes). Once the `chat` resource is healthy, open the `web` app from the dashboard and send a message.

## How It Works

### AppHost — Ollama and the model as resources

```csharp
var ollama = builder.AddOllama("ollama")
    .WithDataVolume();                          // persist downloaded models between runs
    // .WithGPUSupport(OllamaGpuVendor.Nvidia); // enable on a machine with Nvidia drivers

var chat = ollama.AddModel("chat", "llama3.2"); // resource name "chat", model tag "llama3.2"

builder.AddProject<Projects.Web>("web")
    .WithReference(chat)
    .WaitFor(chat);                             // don't start the app until the model is ready
```

### Web — one IChatClient, fed by service discovery

```csharp
builder.AddOllamaApiClient("chat")
    .AddChatClient()
    .UseFunctionInvocation()
    .UseOpenTelemetry(configure: t => t.EnableSensitiveData = builder.Environment.IsDevelopment())
    .UseLogging();
```

The Blazor component only knows `IChatClient` — not Ollama. In production you can swap the registration for a hosted endpoint (e.g. Azure AI Foundry) behind the same interface, without touching the UI.

## Architecture

```
Browser (Blazor chat)
    │  IChatClient.GetStreamingResponseAsync
    ▼
┌─────────────────────┐   service discovery   ┌──────────────────────┐
│  Web (Blazor)       │ ────────────────────► │  ollama (container)  │
│  IChatClient        │ ◄──────────────────── │  model: llama3.2      │
└─────────────────────┘                       └──────────────────────┘
          ▲
          │ orchestrated by (declares resources, pulls model, WaitFor)
┌─────────────────────┐
│  AppHost (.NET Aspire)
└─────────────────────┘
```

Because the chat client runs through `UseOpenTelemetry()`, every model call shows up in the dashboard's **Traces** tab with timing and token counts.

## Troubleshooting

- **First run looks frozen**: it's the model downloading in the background. `.WithDataVolume()` keeps it between runs, so subsequent starts are fast.
- **`pull model manifest ... server misbehaving`**: Docker's container DNS can't reach the registry (common on corporate networks/VPNs). Add public DNS to `~/.docker/daemon.json` and restart Docker Desktop:

  ```json
  { "dns": ["8.8.8.8", "1.1.1.1"] }
  ```

- **Docker not running**: Aspire can't start the Ollama container. Start Docker Desktop first.
- **No GPU**: leave `WithGPUSupport` commented out; Ollama runs on CPU — slower, but fine for development.

## Related Blog Post

Companion code for the blog post: [Orchestrate Blazor + Ollama with .NET Aspire](https://gabrielmongeon.ca/en/2026/06/orchestrate-blazor-ollama-with-aspire/).

Part of the "AI development with Ollama and .NET" series.

## License

See the root [LICENSE](../../../LICENSE) file.
