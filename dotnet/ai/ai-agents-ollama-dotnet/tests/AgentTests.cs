using AIAgentsOllama.Agents;
using AIAgentsOllama.Tools;
using Xunit;

namespace AIAgentsOllama.Tests;

public class AgentTests
{
    [Fact]
    public void Agent_InitializesSuccessfully()
    {
        // Arrange & Act
        var agent = new OllamaAgent("llama3.3");

        // Assert
        Assert.NotNull(agent);
    }

    [Fact]
    public void Agent_RegistersTools()
    {
        // Arrange
        var agent = new OllamaAgent();
        var toolRegistry = new ToolRegistry();
        var tool = new CalculatorTool();

        // Act
        toolRegistry.Register(tool);
        agent.SetTools(toolRegistry);

        // Assert
        var allTools = toolRegistry.GetAllTools();
        Assert.Contains(tool, allTools);
    }

    [Fact]
    public void ToolRegistry_RetrievesTool()
    {
        // Arrange
        var registry = new ToolRegistry();
        var tool = new CalculatorTool();
        registry.Register(tool);

        // Act
        var retrieved = registry.GetTool("calculator");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("calculator", retrieved.Name);
    }

    [Fact]
    public async Task ToolRegistry_ThrowsForMissingTool()
    {
        // Arrange
        var registry = new ToolRegistry();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await registry.ExecuteToolAsync("nonexistent", new())
        );
    }
}
