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
        var outputFile = Path.Combine(Path.GetTempPath(), $"audio_{Guid.NewGuid()}.wav");
        _logger.LogInformation("Enregistrement démarré ({Duration}s)...", _options.RecordingDurationSeconds);

        var psi = new ProcessStartInfo
        {
            FileName = "arecord",
            Arguments = $"-D {_options.AudioDevice} -f cd -t wav -d {_options.RecordingDurationSeconds} {outputFile}",
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(psi)!;
        await process.WaitForExitAsync(cancellationToken);

        _logger.LogInformation("Enregistrement terminé → {File}", outputFile);
        return outputFile;
    }
}