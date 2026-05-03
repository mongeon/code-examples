namespace AudioAssistant.Services.Tools;

public class ToolRegistry
{
    private readonly Dictionary<string, ITool> _tools = new();

    public ToolRegistry(IEnumerable<ITool> tools)
    {
        foreach (var tool in tools)
            _tools[tool.Name] = tool;
    }

    public IEnumerable<ITool> GetAll() => _tools.Values;

    public ITool? GetByName(string name) =>
        _tools.TryGetValue(name, out var tool) ? tool : null;
}