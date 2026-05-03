using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace AudioAssistant.Services;

public interface ISpeechService
{
    Task SpeakAsync(string text, CancellationToken cancellationToken);
}

public class PiperSpeechService : ISpeechService
{
    private readonly AssistantOptions _options;
    private readonly ILogger<PiperSpeechService> _logger;

    public PiperSpeechService(IOptions<AssistantOptions> options, ILogger<PiperSpeechService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SpeakAsync(string text, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Synthèse vocale : \"{Text}\"", text);

        var piperFile = Path.Combine(Path.GetTempPath(), $"tts_{Guid.NewGuid()}.wav");
        var resampledFile = Path.Combine(Path.GetTempPath(), $"tts_resampled_{Guid.NewGuid()}.wav");

        try
        {
            // 1. Piper génère un fichier WAV
            var piperPsi = new ProcessStartInfo
            {
                FileName = _options.PiperBinary,
                Arguments = $"--model {_options.PiperVoice} --output_file {piperFile}",
                RedirectStandardInput = true,
                UseShellExecute = false
            };

            using var piper = Process.Start(piperPsi)!;
            await piper.StandardInput.WriteLineAsync(text);
            piper.StandardInput.Close();
            await piper.WaitForExitAsync(cancellationToken);

            // 2. ffmpeg resample vers 48000Hz stéréo pour l'adaptateur USB
            var ffmpegPsi = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-y -i {piperFile} -ar 48000 -ac 2 {resampledFile}",
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var ffmpeg = Process.Start(ffmpegPsi)!;
            await ffmpeg.WaitForExitAsync(cancellationToken);

            // 3. aplay joue le fichier resamplé
            var aplayPsi = new ProcessStartInfo
            {
                FileName = "aplay",
                Arguments = $"-D {_options.AudioOutputDevice} {resampledFile}",
                UseShellExecute = false
            };

            using var aplay = Process.Start(aplayPsi)!;
            await aplay.WaitForExitAsync(cancellationToken);
        }
        finally
        {
            if (File.Exists(piperFile)) File.Delete(piperFile);
            if (File.Exists(resampledFile)) File.Delete(resampledFile);
        }
    }
}