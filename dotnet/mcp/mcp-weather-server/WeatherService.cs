using System.Net.Http.Json;

namespace McpWeatherServer;

public class WeatherService(HttpClient httpClient)
{
    public async Task<WeatherResult?> GetCurrentWeatherAsync(string city)
    {
        // Step 1: geocoding
        var geoUrl = $"https://geocoding-api.open-meteo.com/v1/search" +
                     $"?name={Uri.EscapeDataString(city)}&count=1&format=json";

        var geoResponse = await httpClient.GetFromJsonAsync<GeocodingResponse>(geoUrl);
        var location = geoResponse?.Results?.FirstOrDefault();
        if (location is null) return null;

        // Step 2: current weather
        var weatherUrl = $"https://api.open-meteo.com/v1/forecast" +
                         $"?latitude={location.Latitude}&longitude={location.Longitude}" +
                         "&current=temperature_2m,apparent_temperature," +
                         "weathercode,windspeed_10m,relative_humidity_2m&timezone=auto";

        var weather = await httpClient.GetFromJsonAsync<OpenMeteoResponse>(weatherUrl);
        if (weather?.Current is null) return null;

        return new WeatherResult(
            City: location.Name,
            Country: location.Country,
            Temperature: weather.Current.Temperature,
            ApparentTemperature: weather.Current.ApparentTemperature,
            Humidity: weather.Current.RelativeHumidity,
            WindSpeed: weather.Current.WindSpeed,
            Condition: WmoCodeToDescription(weather.Current.WeatherCode)
        );
    }

    private static string WmoCodeToDescription(int code) => code switch
    {
        0 => "Clear sky",
        1 => "Mainly clear",
        2 => "Partly cloudy",
        3 => "Overcast",
        45 or 48 => "Fog",
        51 or 53 or 55 => "Drizzle",
        61 or 63 or 65 => "Rain",
        71 or 73 or 75 => "Snow",
        80 or 81 or 82 => "Rain showers",
        95 => "Thunderstorm",
        _ => "Unknown"
    };
}
