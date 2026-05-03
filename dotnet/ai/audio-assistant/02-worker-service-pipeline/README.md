# 02 — Worker Service .NET 10 et pipeline audio

Code exemple pour l'article #2 — structure du projet Worker Service, lecture GPIO, enregistrement audio avec `arecord`, transcription avec Whisper et synthèse vocale avec Piper TTS.

## Prérequis

- Raspberry Pi avec bouton GPIO (pin 17)
- `aplay` et `ffmpeg` installés (`sudo apt install alsa-utils ffmpeg`)
- [Piper TTS](https://github.com/rhasspy/piper) et voix `fr_FR-siwis-low.onnx`
- Adaptateur audio USB configuré (`hw:3,0`)

## Configuration

Ajuster les chemins dans `appsettings.json` selon votre installation (binaire Piper, voix, périphérique audio).

## Exécution

```bash
dotnet run
```
