# Assistant Vocal sur Raspberry Pi — Exemples de code

Exemples de code pour la série d'articles [Assistant Vocal sur Raspberry Pi](https://www.gabrielmongeon.ca/series/assistant-vocal-sur-raspberry-pi/) sur [gabrielmongeon.ca](https://www.gabrielmongeon.ca).

> English version: [Voice Assistant on Raspberry Pi](https://www.gabrielmongeon.ca/en/series/voice-assistant-on-raspberry-pi/)

## Structure

| Dossier | Article |
|---------|---------|
| `02-worker-service-pipeline` | Worker Service .NET 10 et pipeline audio |
| `03-ollama-context` | Intégration Ollama et contexte maison |
| `04-memory-silence-systemd` | Mémoire, détection de silence et systemd |
| `05-weather-claude-api` | Météo en temps réel et swap Claude API |
| `06-function-calling` | Function Calling — enseigner des outils à l'assistant |

## Prérequis matériels

- Raspberry Pi (testé sur Raspberry Pi 5)
- Bouton physique connecté au GPIO (pin 17 par défaut)
- Adaptateur USB audio
- Micro et haut-parleur

## Prérequis logiciels

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Ollama](https://ollama.com) (articles #3 et #4) — modèle `llama3.2:1b` recommandé
- [Piper TTS](https://github.com/rhasspy/piper) — voix française `fr_FR-siwis-low`
- [Whisper.net](https://github.com/sandrohanea/whisper.net) — modèle `ggml-base.bin` (téléchargé automatiquement)
- `ffmpeg` et `aplay` (paquets système Linux)
- Clé API Anthropic (articles #5 et #6) — [console.anthropic.com](https://console.anthropic.com)

> Ces projets sont conçus pour tourner sur Raspberry Pi OS Lite (Linux/ARM). La compilation fonctionne sur toutes les plateformes, mais l'exécution requiert le matériel et les dépendances Linux listés ci-dessus.
