using System.ComponentModel;
using Microsoft.Extensions.AI;
using OllamaSharp;

// OllamaApiClient implements IChatClient, so everything plugs in directly
IChatClient client = new OllamaApiClient(
    new Uri("http://localhost:11434"), "llama3.2");

// --- 1. Typed extraction with GetResponseAsync<T> ---
// The JSON schema is generated from the Invoice type, sent in Ollama's
// "format" field, and the response is deserialized into a C# object.
var response = await client.GetResponseAsync<Invoice>(
    "Extract the info (date in YYYY-MM-DD format): " +
    "Invoice from Hydro-Quebec, $142.50, June 15 2026",
    new ChatOptions { Temperature = 0 }); // temperature at 0 for more stable results

Invoice invoice = response.Result;
Console.WriteLine($"{invoice.Vendor}: ${invoice.Amount} on {invoice.Date}");

// --- 2. Defensive variant: TryGetResult + content validation ---
// The schema guarantees the structure, but not the content. A small model
// forced to fill a required field will fill it even without the information,
// so validate the content on the C# side, like any external input.
var second = await client.GetResponseAsync<Invoice>(
    "Extract the info (date in YYYY-MM-DD format): " +
    "Payment reminder from Bell Canada, $89.99, July 2 2026",
    new ChatOptions { Temperature = 0 });

if (second.TryGetResult(out var result) && IsValid(result))
{
    Console.WriteLine($"{result.Vendor}: ${result.Amount} on {result.Date}");
}
else
{
    // Log the raw response and handle the failure
    Console.WriteLine($"Unusable response, raw text: {second.Text}");
}

static bool IsValid(Invoice invoice) =>
    !string.IsNullOrWhiteSpace(invoice.Vendor)
    && invoice.Amount > 0
    && DateOnly.TryParseExact(invoice.Date, "yyyy-MM-dd", out _);

// The Description attributes are included in the schema sent to the model
public record Invoice(
    [property: Description("The vendor name")] string Vendor,
    [property: Description("The total amount, taxes included")] decimal Amount,
    [property: Description("The date in YYYY-MM-DD format")] string Date);
