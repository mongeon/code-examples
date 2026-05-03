using AudioAssistant;
using AudioAssistant.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<AssistantOptions>(
    builder.Configuration.GetSection("Assistant"));

builder.Services.AddSingleton<IGpioService, GpioService>();
builder.Services.AddSingleton<IAudioRecorderService, AudioRecorderService>();
builder.Services.AddSingleton<ITranscriptionService, WhisperTranscriptionService>();
builder.Services.AddSingleton<ISpeechService, PiperSpeechService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();