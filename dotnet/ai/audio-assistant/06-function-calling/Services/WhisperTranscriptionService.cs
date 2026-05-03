using Microsoft.Extensions.Options;
using Whisper.net;
using Whisper.net.Ggml;

namespace AudioAssistant.Services;

public interface ITranscriptionService
{
    Task<string> TranscribeAsync(string audioFilePath, CancellationToken cancellationToken);
}

public class WhisperTranscriptionService : ITranscriptionService
{
    private readonly string _modelPath;
    private readonly ILogger<WhisperTranscriptionService> _logger;

    public WhisperTranscriptionService(IOptions<AssistantOptions> options, ILogger<WhisperTranscriptionService> logger)
    {
        _modelPath = options.Value.WhisperModel;
        _logger = logger;
    }

    public async Task<string> TranscribeAsync(string audioFilePath, CancellationToken cancellationToken)
    {
        // Télécharger le modèle si absent
        if (!File.Exists(_modelPath))
        {
            _logger.LogInformation("Téléchargement du modèle Whisper...");
            var downloader = new WhisperGgmlDownloader(new HttpClient());
            using var modelStream = await downloader.GetGgmlModelAsync(GgmlType.Base);
            using var fileStream = File.OpenWrite(_modelPath);
            await modelStream.CopyToAsync(fileStream, cancellationToken);
        }

        _logger.LogInformation("Transcription en cours...");

        using var factory = WhisperFactory.FromPath(_modelPath);
        using var processor = factory.CreateBuilder()
            .WithLanguage("fr")
            .Build();

        var result = new System.Text.StringBuilder();

        // ProcessAsync prend un Stream, pas un chemin
        using var audioStream = File.OpenRead(audioFilePath);
        await foreach (var segment in processor.ProcessAsync(audioStream, cancellationToken))
        {
            result.Append(segment.Text);
        }

        var text = result.ToString().Trim();
        _logger.LogInformation("Transcription : \"{Text}\"", text);
        return text;
    }
}