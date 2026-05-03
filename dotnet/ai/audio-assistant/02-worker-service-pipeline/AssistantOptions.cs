namespace AudioAssistant;

public class AssistantOptions
{
    public int GpioButtonPin { get; set; } = 17;
    public string AudioDevice { get; set; } = "hw:1,0";
    public int RecordingDurationSeconds { get; set; } = 10;
    public string WhisperModel { get; set; } = "ggml-base.bin";
    public string PiperBinary { get; set; } = "/home/gabriel/piper/piper/piper";
    public string PiperVoice { get; set; } = "/home/gabriel/piper-voices/fr_FR-siwis-low.onnx";
    public string AudioOutputDevice { get; set; } = "default";
}