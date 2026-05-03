using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace AudioAssistant.Services;

public interface IAudioRecorderService
{
    Task<string> RecordAsync(CancellationToken cancellationToken);
}

public class AudioRecorderService : IAudioRecorderService
{
    private readonly AssistantOptions _options;
    private readonly ILogger<AudioRecorderService> _logger;

    public AudioRecorderService(IOptions<AssistantOptions> options, ILogger<AudioRecorderService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> RecordAsync(CancellationToken cancellationToken)
    {
        var rawFile = Path.Combine(Path.GetTempPath(), $"audio_raw_{Guid.NewGuid()}.wav");
        var outputFile = Path.Combine(Path.GetTempPath(), $"audio_{Guid.NewGuid()}.wav");

        _logger.LogInformation("Enregistrement démarré (silence auto)...");

        // Enregistrer avec durée max de sécurité (RecordingDurationSeconds)
        // ffmpeg coupe automatiquement après SilenceDurationMs de silence
        var ffmpegPsi = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = string.Join(" ",
                "-f alsa",
                $"-i {_options.AudioDevice}",
                "-af",
                $"silencedetect=noise={_options.SilenceThreshold}:d={_options.SilenceDurationMs / 1000.0}",
                $"-t {_options.RecordingDurationSeconds}",
                "-ar 16000 -ac 1",
                $"-y {rawFile}"),
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var ffmpegProcess = Process.Start(ffmpegPsi)!;

        // Lire stderr pour détecter la fin du silence
        _ = Task.Run(async () =>
        {
            string? line;
            while ((line = await ffmpegProcess.StandardError.ReadLineAsync()) != null)
            {
                if (line.Contains("silence_end"))
                {
                    _logger.LogInformation("Silence détecté — arrêt de l'enregistrement.");
                    // Envoyer q pour arrêt propre au lieu de Kill
                    ffmpegProcess.StandardInput.Write("q");
                    break;
                }
            }
        }, cancellationToken);

        await ffmpegProcess.WaitForExitAsync(cancellationToken);

        // Convertir en 16kHz mono pour Whisper si nécessaire
        var convertPsi = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-y -i {rawFile} -ar 16000 -ac 1 {outputFile}",
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false
        };

        using var convertProcess = Process.Start(convertPsi)!;
        await convertProcess.WaitForExitAsync(cancellationToken);

        if (File.Exists(rawFile))
            File.Delete(rawFile);

        _logger.LogInformation("Enregistrement terminé → {File}", outputFile);
        return outputFile;
    }
}