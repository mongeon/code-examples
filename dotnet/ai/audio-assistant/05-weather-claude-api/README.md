# 05 — Météo en temps réel et swap Claude API

Code exemple pour l'article #5 — ajout de `WeatherService` (Open-Meteo), de `ClaudeService` via le SDK Anthropic, et pattern factory `LlmProvider` pour basculer entre Ollama et Claude.

## Prérequis

- Tout ce qui est requis à l'article #3
- Clé API Anthropic — [console.anthropic.com](https://console.anthropic.com)

## Configuration

Dans `appsettings.json` :
- `LlmProvider` : `"claude"` ou `"ollama"`
- `ClaudeApiKey` : remplacer `YOUR_CLAUDE_API_KEY_HERE` par votre clé
- `WeatherLatitude` / `WeatherLongitude` : coordonnées de votre localité

## Exécution

```bash
dotnet run
```
