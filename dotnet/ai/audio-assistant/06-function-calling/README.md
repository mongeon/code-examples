# 06 — Function Calling

Code exemple pour l'article #6 — système d'outils extensible avec interface `ITool`, `ToolRegistry`, et implémentations `WeatherTool` (Open-Meteo) et `TimeTool`.

## Prérequis

- Tout ce qui est requis à l'article #5

## Ajouter un outil

Implémenter l'interface `ITool` dans `Services/Tools/` et l'enregistrer dans `Program.cs` via `ToolRegistry`.

## Configuration

Même configuration que l'article #5 (`appsettings.json`).

## Exécution

```bash
dotnet run
```
