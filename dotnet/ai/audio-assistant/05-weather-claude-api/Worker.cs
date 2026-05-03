using AudioAssistant.Services;

namespace AudioAssistant;

public class Worker : BackgroundService
{
    private readonly IGpioService _gpio;
    private readonly IAudioRecorderService _recorder;
    private readonly ITranscriptionService _transcription;
    private readonly ILlmService _llm;
    private readonly IContextService _context;
    private readonly ISpeechService _speech;
    private readonly ILogger<Worker> _logger;

    public Worker(
        IGpioService gpio,
        IAudioRecorderService recorder,
        ITranscriptionService transcription,
        ILlmService llm,
        IContextService context,
        ISpeechService speech,
        ILogger<Worker> logger)
    {
        _gpio = gpio;
        _recorder = recorder;
        _transcription = transcription;
        _llm = llm;
        _context = context;
        _speech = speech;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Assistant démarré. Appuie sur le bouton pour parler.");

        // Initialiser le contexte avec la météo actuelle
        await _context.InitializeAsync(stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _gpio.WaitForButtonPress(stoppingToken);
            if (stoppingToken.IsCancellationRequested) break;

            try
            {
                // 1. Enregistrer
                var audioFile = await _recorder.RecordAsync(stoppingToken);

                // 2. Transcrire
                var texte = await _transcription.TranscribeAsync(audioFile, stoppingToken);

                if (string.IsNullOrWhiteSpace(texte))
                {
                    await _speech.SpeakAsync("Je n'ai pas bien entendu. Peux-tu répéter?", stoppingToken);
                }
                else
                {
                    // 3. Ajouter au contexte et appeler le LLM
                    var history = _context.AddUserMessage(texte);
                    var reponse = await _llm.ChatAsync(history, stoppingToken);

                    // 4. Mémoriser la réponse
                    _context.AddAssistantMessage(reponse);

                    // 5. Synthèse vocale
                    await _speech.SpeakAsync(reponse, stoppingToken);
                }

                if (File.Exists(audioFile))
                    File.Delete(audioFile);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Erreur dans le pipeline");
                await _speech.SpeakAsync("Une erreur s'est produite.", stoppingToken);
            }
        }
    }
}