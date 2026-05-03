using AudioAssistant.Services;

namespace AudioAssistant;

public class Worker : BackgroundService
{
    private readonly IGpioService _gpio;
    private readonly IAudioRecorderService _recorder;
    private readonly ITranscriptionService _transcription;
    private readonly ISpeechService _speech;
    private readonly ILogger<Worker> _logger;

    public Worker(
        IGpioService gpio,
        IAudioRecorderService recorder,
        ITranscriptionService transcription,
        ISpeechService speech,
        ILogger<Worker> logger)
    {
        _gpio = gpio;
        _recorder = recorder;
        _transcription = transcription;
        _speech = speech;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Assistant démarré. Appuie sur le bouton pour parler.");

        while (!stoppingToken.IsCancellationRequested)
        {
            // 1. Attendre la pression du bouton
            _gpio.WaitForButtonPress(stoppingToken);
            if (stoppingToken.IsCancellationRequested) break;

            try
            {
                // 2. Enregistrer
                var audioFile = await _recorder.RecordAsync(stoppingToken);

                // 3. Transcrire
                var texte = await _transcription.TranscribeAsync(audioFile, stoppingToken);

                // 4. Réponse hardcodée (le LLM arrive à l'article #3)
                var reponse = string.IsNullOrWhiteSpace(texte)
                    ? "Je n'ai pas bien entendu. Peux-tu répéter?"
                    : $"Tu as dit : {texte}. Je suis encore en construction, reviens bientôt!";

                // 5. Synthèse vocale
                await _speech.SpeakAsync(reponse, stoppingToken);

                // Nettoyage du fichier temporaire
                if (File.Exists(audioFile))
                    File.Delete(audioFile);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Erreur dans le pipeline audio");
                await _speech.SpeakAsync("Une erreur s'est produite.", stoppingToken);
            }
        }
    }
}