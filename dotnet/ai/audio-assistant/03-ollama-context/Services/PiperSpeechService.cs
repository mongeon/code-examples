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

        // Piper génère du raw audio → on pipe vers aplay
            var piperPsi = new ProcessStartInfo
            {
                FileName = _options.PiperBinary,
                Arguments = $"--model {_options.PiperVoice} --output_raw",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var aplayPsi = new ProcessStartInfo
            {
                FileName = "aplay",
                Arguments = $"-D {_options.AudioOutputDevice} -r 22050 -f S16_LE -c 2 -t raw -",
                RedirectStandardInput = true,
                UseShellExecute = false
            };

        using var piper = Process.Start(piperPsi)!;
        using var aplay = Process.Start(aplayPsi)!;

        // Écrire le texte dans Piper
        await piper.StandardInput.WriteLineAsync(text);
        piper.StandardInput.Close();

        // Pipe la sortie de Piper vers aplay
        await piper.StandardOutput.BaseStream.CopyToAsync(
            aplay.StandardInput.BaseStream, cancellationToken);
        aplay.StandardInput.Close();

        await Task.WhenAll(
            piper.WaitForExitAsync(cancellationToken),
            aplay.WaitForExitAsync(cancellationToken));
    }
}