# Structured Outputs with Ollama and .NET

A minimal console app showing how to get reliable, typed JSON from a local model with Ollama's `format` field and `Microsoft.Extensions.AI`'s `GetResponseAsync<T>`.

Companion code for the blog post: [Structured Outputs with Ollama and .NET](https://www.gabrielmongeon.ca/en/blog/structured-outputs-ollama-dotnet)

## Project Structure

```
structured-outputs-ollama-dotnet/
  StructuredOutputsOllama.csproj   # Project file (net10.0, Microsoft.Extensions.AI + OllamaSharp)
  Program.cs                       # Typed extraction, TryGetResult and content validation
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Ollama](https://ollama.com/) 0.5 or later, with a model pulled:

```bash
ollama pull llama3.2
```

## Running

Make sure Ollama is running locally (`http://localhost:11434`), then, from this folder:

```bash
dotnet run
```

Expected output (values may vary slightly by model, and decimal formatting follows your system culture):

```
Hydro-Quebec: $142.50 on 2026-06-15
Bell Canada: $89.99 on 2026-07-02
```

## Notes

- `GetResponseAsync<T>` generates the JSON schema from the C# type and sends it in Ollama's `format` field. The schema is enforced at the decoding level, so the response is always conforming JSON — no code fences, no surrounding text.
- The schema guarantees the structure, but not the content: keep validating on the C# side, like any external input.
- Streaming brings nothing with a typed output: a half-deserialized object is useless, so `GetResponseAsync<T>` waits for the complete response.
