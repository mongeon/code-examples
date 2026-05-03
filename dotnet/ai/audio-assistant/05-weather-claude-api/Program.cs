using AudioAssistant;
using AudioAssistant.Services;
using Microsoft.Extensions.Options; 

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<AssistantOptions>(
    builder.Configuration.GetSection("Assistant"));

builder.Services.AddSingleton<IGpioService, GpioService>();
builder.Services.AddSingleton<IAudioRecorderService, AudioRecorderService>();
builder.Services.AddSingleton<ITranscriptionService, WhisperTranscriptionService>();
builder.Services.AddSingleton<ISpeechService, PiperSpeechService>();
builder.Services.AddSingleton<IContextService, ContextService>();

builder.Services.AddHttpClient<IWeatherService, WeatherService>();

// Enregistrer les deux services LLM
builder.Services.AddHttpClient<OllamaService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(120);
});
builder.Services.AddTransient<ClaudeService>();

// Factory qui choisit selon LlmProvider dans appsettings
builder.Services.AddTransient<ILlmService>(sp =>
{
    var options = sp.GetRequiredService<IOptions<AssistantOptions>>().Value;
    if (options.LlmProvider.Equals("claude", StringComparison.OrdinalIgnoreCase))
        return sp.GetRequiredService<ClaudeService>();
    return sp.GetRequiredService<OllamaService>();
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();