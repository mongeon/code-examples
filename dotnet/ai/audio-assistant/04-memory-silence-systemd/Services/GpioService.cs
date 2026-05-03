using System.Device.Gpio;
using Microsoft.Extensions.Options;

namespace AudioAssistant.Services;

public interface IGpioService : IDisposable
{
    bool IsButtonPressed();
    void WaitForButtonPress(CancellationToken cancellationToken);
}

public class GpioService : IGpioService
{
    private readonly GpioController _gpio;
    private readonly int _buttonPin;
    private readonly ILogger<GpioService> _logger;

    public GpioService(IOptions<AssistantOptions> options, ILogger<GpioService> logger)
    {
        _buttonPin = options.Value.GpioButtonPin;
        _logger = logger;
        _gpio = new GpioController();
        _gpio.OpenPin(_buttonPin, PinMode.InputPullUp);
        _logger.LogInformation("GPIO initialisé sur le pin {Pin}", _buttonPin);
    }

    public bool IsButtonPressed()
        => _gpio.Read(_buttonPin) == PinValue.Low;

    public void WaitForButtonPress(CancellationToken cancellationToken)
    {
        _logger.LogInformation("En attente d'une pression sur le bouton...");
        while (!cancellationToken.IsCancellationRequested)
        {
            if (IsButtonPressed())
                return;
            Thread.Sleep(50);
        }
    }

    public void Dispose() => _gpio.Dispose();
}