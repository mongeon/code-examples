# 03 — Intégration Ollama et contexte maison

Code exemple pour l'article #3 — ajout d'`OllamaService` pour la génération LLM locale et de `ContextService` pour maintenir l'historique conversationnel.

## Prérequis

- Tout ce qui est requis à l'article #2
- [Ollama](https://ollama.com) installé et accessible sur le réseau local
- Modèle téléchargé : `ollama pull llama3.2:1b`

## Configuration

Dans `appsettings.json`, ajuster `OllamaBaseUrl` (ex: `http://10.0.0.x:11434`) et `SystemPrompt` selon vos besoins.

## Exécution

```bash
dotnet run
```
