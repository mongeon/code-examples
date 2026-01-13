using NCalc;

namespace AIAgentsOllama.Tools;

/// <summary>
/// Tool for mathematical calculations.
/// </summary>
public class CalculatorTool : ITool
{
    public string Name => "calculator";
    public string Description => "Performs mathematical calculations. Input: expression (e.g., '15% of 2450' or '(15/100)*2450')";

    public Task<string> ExecuteAsync(Dictionary<string, object> parameters)
    {
        if (!parameters.TryGetValue("expression", out var expression) || expression == null)
            return Task.FromResult("Error: 'expression' parameter is required");

        try
        {
            var expr = new Expression(expression.ToString()!);
            var result = expr.Evaluate();
            return Task.FromResult(result?.ToString() ?? "No result");
        }
        catch (Exception ex)
        {
            return Task.FromResult($"Error: {ex.Message}");
        }
    }
}
