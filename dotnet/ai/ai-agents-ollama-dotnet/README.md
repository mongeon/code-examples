# AI Agents with Ollama and .NET

A complete, working example of building autonomous AI agents using Ollama and .NET. This project demonstrates the ReAct pattern, tool calling, and practical agent architectures.

## Features

- **Tool-Based Agents**: Register and use custom tools for agent execution
- **ReAct Pattern**: Alternating reasoning and acting loop
- **Multi-Agent System**: Specialized agents with different roles
- **RAG Integration**: Combine agents with knowledge retrieval
- **Memory Management**: Short-term, long-term, and working memory

## Project Structure

```
ai-agents-ollama-dotnet/
├── src/
│   ├── Agents/
│   │   ├── OllamaAgent.cs                    # Core agent implementation
│   │   ├── RAGAgent.cs                       # Agent with RAG capabilities
│   │   ├── MultiAgentSystem.cs               # Multi-agent coordination
│   │   └── AutonomousAgent.cs                # Self-assessing agent
│   ├── Tools/
│   │   ├── ITool.cs                          # Tool interface
│   │   ├── ToolRegistry.cs                   # Tool management
│   │   ├── CalculatorTool.cs                 # Math calculations
│   │   ├── WebSearchTool.cs                  # Web search (mock)
│   │   ├── CodeExecutionTool.cs              # Safe code execution
│   │   └── KnowledgeBaseTool.cs              # RAG knowledge search
│   ├── Models/
│   │   ├── ChatMessage.cs                    # Message model
│   │   ├── ToolCall.cs                       # Tool invocation model
│   │   └── AgentResponse.cs                  # Agent response model
│   └── Utilities/
│       └── OllamaClient.cs                   # HTTP client wrapper
├── examples/
│   ├── BasicAgentExample.cs                  # Simple agent usage
│   ├── CalculatorAgentExample.cs             # Math & search agent
│   ├── CodeAnalysisAgentExample.cs           # Code analysis agent
│   └── RAGAgentExample.cs                    # Agent with knowledge base
├── tests/
│   ├── AgentTests.cs                         # Agent behavior tests
│   ├── ToolTests.cs                          # Tool execution tests
│   └── MockTools.cs                          # Mock implementations for testing
├── AIAgentsOllama.csproj                     # Project file
└── .editorconfig
```

## Prerequisites

- .NET 8.0 or higher
- Ollama (download from https://ollama.ai)
- A running Ollama instance with a model (e.g., `ollama pull llama3.3`)

## Installation

1. Clone the repository
2. Navigate to this directory
3. Restore dependencies:
   ```bash
   dotnet restore
   ```

## Usage

### Running Examples

```bash
# Run basic agent example
dotnet run --project examples/BasicAgentExample.cs

# Run calculator agent
dotnet run --project examples/CalculatorAgentExample.cs

# Run code analysis agent
dotnet run --project examples/CodeAnalysisAgentExample.cs
```

### Creating Your Own Agent

```csharp
using AIAgentsOllama.Agents;
using AIAgentsOllama.Tools;

// Create agent instance
var agent = new OllamaAgent("llama3.3", "http://localhost:11434");

// Register tools
var toolRegistry = new ToolRegistry();
toolRegistry.Register(new CalculatorTool());
toolRegistry.Register(new WebSearchTool());
agent.SetTools(toolRegistry);

// Run agent
var result = await agent.RunAsync(
    "What is 15% of 2,450? Then search for the current USD to CAD exchange rate.",
    maxIterations: 10
);

Console.WriteLine(result);
```

## Core Concepts

### 1. Tools

Tools are capabilities that agents can invoke:

```csharp
public interface ITool
{
    string Name { get; }
    string Description { get; }
    Task<string> ExecuteAsync(Dictionary<string, object> parameters);
}
```

### 2. Agent Loop

The ReAct pattern:
1. **Thought**: LLM analyzes the problem
2. **Action**: Agent calls a tool
3. **Observation**: Process the tool result
4. **Repeat** or finish

### 3. Memory Systems

- **Conversation History**: Chat messages for context
- **Working Memory**: Temporary state during execution
- **Knowledge Base**: Integration with RAG for domain expertise

## Configuration

Edit configuration in your code:

```csharp
var agent = new OllamaAgent(
    modelName: "llama3.3",        // Model to use
    ollamaUrl: "http://localhost:11434",  // Ollama server
    timeout: TimeSpan.FromMinutes(5)      // Request timeout
);
```

## Testing

Run tests with:

```bash
dotnet test
```

Tests include:
- Agent decision-making logic
- Tool execution and error handling
- Multi-agent coordination
- Memory management

## Real-World Use Cases

### Code Analysis Agent
Analyze code for security vulnerabilities and suggest fixes.

### DevOps Agent
Monitor production systems and take corrective actions.

### Customer Support Agent
Handle support tickets with access to knowledge bases.

### Research Agent
Combine web search, code execution, and RAG for complex research.

## Performance Tips

1. **Model Selection**: Use appropriate models for your task
   - Simple: `llama3.2:3b`
   - Complex reasoning: `llama3.3:70b`
   - Code: `deepseek-coder`

2. **Caching**: Cache tool results to avoid redundant calls

3. **Timeouts**: Set reasonable timeouts to prevent infinite loops

4. **Tool Design**: Make tool descriptions clear and specific

## Troubleshooting

### Agent doesn't call tools
- Ensure tool descriptions are clear and specific
- Check if the model supports function calling (use recent models)
- Verify tool parameters match the model's expected format

### Ollama connection errors
- Ensure Ollama is running: `ollama serve`
- Check the URL: default is `http://localhost:11434`
- Verify the model is pulled: `ollama list`

### Memory issues with large models
- Use smaller models for testing (`llama3.2:3b`)
- Reduce max iterations
- Run on a machine with sufficient RAM/VRAM

## Resources

- [Blog Post: Building AI Agents with Ollama and .NET](https://www.gabrielmongeon.ca/blog/ai-agents-ollama-dotnet)
- [Ollama Documentation](https://github.com/ollama/ollama)
- [ReAct Paper](https://arxiv.org/abs/2210.03629)
- [Function Calling Guide](https://platform.openai.com/docs/guides/function-calling)

## License

MIT License - See LICENSE file in repository root

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.
