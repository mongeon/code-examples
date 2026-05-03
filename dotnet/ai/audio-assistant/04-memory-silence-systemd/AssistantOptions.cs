namespace AudioAssistant;

public class AssistantOptions
{
    public int GpioButtonPin { get; set; } = 17;
    public string AudioDevice { get; set; } = "hw:3,0";
    public int RecordingDurationSeconds { get; set; } = 10;
    public string WhisperModel { get; set; } = "ggml-base.bin";
    public string PiperBinary { get; set; } = "/home/gabriel/piper/piper/piper";
    public string PiperVoice { get; set; } = "/home/gabriel/piper-voices/fr_FR-siwis-low.onnx";
    public string AudioOutputDevice { get; set; } = "hw:3,0";
    public string OllamaBaseUrl { get; set; } = "http://pi-cerveau.local:11434";
    public string OllamaModel { get; set; } = "llama3.2:3b";
    public string SystemPrompt { get; set; } = "";
    public int MaxConversationTurns { get; set; } = 10;
    public int SilenceDurationMs { get; set; } = 1500;   // 1.5s de silence pour couper
    public string SilenceThreshold { get; set; } = "-40dB"; // Seuil de détection
}