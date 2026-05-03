using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using Anthropic.SDK.Messaging;
using System.Text.Json.Nodes;

namespace AudioAssistant.Services.Tools;

public class WeatherTool : ITool
{
    private readonly IWeatherService _weather;

    public WeatherTool(IWeatherService weather)
    {
        _weather = weather;
    }

    public string Name => "get_weather";

    public string Description =>
        "Obtient la météo actuelle pour une ville donnée. Si aucune ville n'est mentionnée, " +
        "utilise la localisation par défaut. Appelle cet outil quand l'utilisateur pose des " +
        "questions sur la météo, comment s'habiller, s'il faut prendre un parapluie, " +
        "sortir les enfants, ou planifier une activité extérieure.";

    public InputSchema InputSchema => new InputSchema
    {
        Type = "object",
        Properties = new Dictionary<string, Property>
        {
            ["city"] = new Property
            {
                Type = "string",
                Description = "Nom de la ville (ex: 'Montréal', 'Québec', 'Toronto'). " +
                            "Laisser vide pour utiliser la localisation par défaut."
            }
        },
        Required = Array.Empty<string>()
    };

    public async Task<string> ExecuteAsync(string input, CancellationToken cancellationToken)
    {
        // Extraire la ville du JSON passé par le LLM
        string? city = null;
        if (!string.IsNullOrWhiteSpace(input))
        {
            try
            {
                var args = JsonSerializer.Deserialize<WeatherToolInput>(input);
                city = args?.City;
            }
            catch { /* pas de paramètres, on utilise la valeur par défaut */ }
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            // Géocoder la ville via Open-Meteo Geocoding API
            var coords = await GeocodeAsync(city, cancellationToken);
            if (coords.HasValue)
                return await _weather.GetCurrentWeatherAsync(
                    cancellationToken, coords.Value.lat, coords.Value.lon, city);
        }

        // Fallback sur les coordonnées par défaut
        return await _weather.GetCurrentWeatherAsync(cancellationToken);
    }

    private async Task<(double lat, double lon)?> GeocodeAsync(
        string city, CancellationToken cancellationToken)
    {
        try
        {
            using var http = new HttpClient();
            var url = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(city)}&count=1&language=fr";
            var response = await http.GetFromJsonAsync<GeocodingResponse>(url, cancellationToken);
            var result = response?.Results?.FirstOrDefault();
            if (result != null)
                return (result.Latitude, result.Longitude);
        }
        catch { /* géocodage échoué, fallback */ }
        return null;
    }
    
    public JsonObject GetParametersSchema() => new JsonObject
    {
        ["type"] = "object",
        ["properties"] = new JsonObject
        {
            ["city"] = new JsonObject
            {
                ["type"] = "string",
                ["description"] = "Nom de la ville. Laisser vide pour utiliser la localisation par défaut."
            }
        },
        ["required"] = new JsonArray()
};
}

internal record WeatherToolInput(
    [property: JsonPropertyName("city")] string? City);

internal record GeocodingResponse(
    [property: JsonPropertyName("results")] List<GeocodingResult>? Results);

internal record GeocodingResult(
    [property: JsonPropertyName("latitude")] double Latitude,
    [property: JsonPropertyName("longitude")] double Longitude);