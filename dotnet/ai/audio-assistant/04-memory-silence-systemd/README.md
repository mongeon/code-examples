# 04 — Mémoire, détection de silence et systemd

Code exemple pour l'article #4 — historique conversationnel limité (`MaxConversationTurns`), détection automatique du silence via `ffmpeg` et configuration pour démarrage automatique avec systemd.

## Prérequis

- Tout ce qui est requis à l'article #3

## Configuration

Dans `appsettings.json`, ajuster `SilenceDurationMs`, `SilenceThreshold` et `MaxConversationTurns` selon vos préférences.

## Exécution

```bash
dotnet run
```
