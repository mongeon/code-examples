using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace AudioAssistant.Services;

public interface IWeatherService
{
    Task<string> GetCurrentWeatherAsync(CancellationToken cancellationToken,
        double? latitude = null, double? longitude = null, string? locationName = null);
}

public class WeatherService : IWeatherService
{
    private readonly HttpClient _http;
    private readonly AssistantOptions _options;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(HttpClient http, IOptions<AssistantOptions> options, ILogger<WeatherService> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GetCurrentWeatherAsync(CancellationToken cancellationToken,
        double? latitude = null, double? longitude = null, string? locationName = null)
    {
        var lat = latitude ?? _options.WeatherLatitude;
        var lon = longitude ?? _options.WeatherLongitude;
        var name = locationName ?? _options.WeatherLocationName; 
        try
        {
            var url = $"https://api.open-meteo.com/v1/forecast" +
                $"?latitude={lat}" +
                $"&longitude={lon}" +
                $"&current=temperature_2m,apparent_temperature,precipitation,weathercode,windspeed_10m" +
                $"&timezone=America%2FToronto" +
                $"&forecast_days=1";

            var response = await _http.GetFromJsonAsync<OpenMeteoResponse>(url, cancellationToken);

            if (response?.Current == null)
                return "Météo non disponible.";

            var description = GetWeatherDescription(response.Current.Weathercode);
            var summary = $"{description}, {response.Current.Temperature2m:F1}°C " +
                         $"(ressenti {response.Current.ApparentTemperature:F1}°C), " +
                         $"vent {response.Current.Windspeed10m:F0} km/h";

            if (response.Current.Precipitation > 0)
                summary += $", précipitations {response.Current.Precipitation:F1} mm";

            _logger.LogInformation("Météo {Location} : {Summary}", name, summary);
            return $"{name} : {summary}";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Impossible d'obtenir la météo.");
            return "Météo non disponible.";
        }
    }

    private static string GetWeatherDescription(int code) => code switch
    {
        0 => "Ciel dégagé",
        1 => "Principalement dégagé",
        2 => "Partiellement nuageux",
        3 => "Couvert",
        45 or 48 => "Brouillard",
        51 or 53 or 55 => "Bruine",
        61 or 63 or 65 => "Pluie",
        71 or 73 or 75 => "Neige",
        77 => "Grains de neige",
        80 or 81 or 82 => "Averses de pluie",
        85 or 86 => "Averses de neige",
        95 => "Orage",
        96 or 99 => "Orage avec grêle",
        _ => "Conditions variables"
    };
}


// DTOs Open-Meteo
internal record OpenMeteoResponse(
    [property: JsonPropertyName("current")] OpenMeteoCurrent Current);

internal record OpenMeteoCurrent(
    [property: JsonPropertyName("temperature_2m")] double Temperature2m,
    [property: JsonPropertyName("apparent_temperature")] double ApparentTemperature,
    [property: JsonPropertyName("precipitation")] double Precipitation,
    [property: JsonPropertyName("weathercode")] int Weathercode,
    [property: JsonPropertyName("windspeed_10m")] double Windspeed10m);