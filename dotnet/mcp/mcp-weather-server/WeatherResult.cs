namespace McpWeatherServer;

public record WeatherResult(
    string City,
    string Country,
    double Temperature,
    double ApparentTemperature,
    int Humidity,
    double WindSpeed,
    string Condition
);
