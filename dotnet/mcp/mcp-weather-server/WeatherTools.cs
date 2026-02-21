using System.ComponentModel;
using ModelContextProtocol.Server;

namespace McpWeatherServer;

[McpServerToolType]
public static class WeatherTools
{
    [McpServerTool]
    [Description("Gets current weather conditions for a city using the Open-Meteo API.")]
    public static async Task<string> GetCurrentWeather(
        WeatherService weatherService,
        [Description("City name (e.g. 'Montreal', 'Paris', 'London')")]
        string city)
    {
        var weather = await weatherService.GetCurrentWeatherAsync(city);

        if (weather is null)
            return $"Could not find weather data for '{city}'. Check the city name and try again.";

        return $"""
            Weather for {weather.City}, {weather.Country}:
            - Condition: {weather.Condition}
            - Temperature: {weather.Temperature}°C (feels like {weather.ApparentTemperature}°C)
            - Humidity: {weather.Humidity}%
            - Wind: {weather.WindSpeed} km/h
            """;
    }
}
