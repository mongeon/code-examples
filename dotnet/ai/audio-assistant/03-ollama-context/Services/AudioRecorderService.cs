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
        
        _logger.LogInformation("Enregistrement démarré ({Duration}s)...", _options.RecordingDurationSeconds);

        // Enregistrer en natif (48000Hz stéréo)
        var recordPsi = new ProcessStartInfo
        {
            FileName = "arecord",
            Arguments = $"-D {_options.AudioDevice} -f S16_LE -c 2 -t wav -d {_options.RecordingDurationSeconds} {rawFile}",
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var recordProcess = Process.Start(recordPsi)!;
        await recordProcess.WaitForExitAsync(cancellationToken);

        // Convertir en 16000Hz mono pour Whisper
        var ffmpegPsi = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-y -i {rawFile} -ar 16000 -ac 1 {outputFile}",
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var ffmpegProcess = Process.Start(ffmpegPsi)!;
        await ffmpegProcess.WaitForExitAsync(cancellationToken);

        if (File.Exists(rawFile))
            File.Delete(rawFile);

        _logger.LogInformation("Enregistrement terminé → {File}", outputFile);
        return outputFile;
    }
}