var builder = DistributedApplication.CreateBuilder(args);

// The Ollama server runs in a container managed by Aspire.
var ollama = builder.AddOllama("ollama")
    .WithDataVolume();                          // persist downloaded models between runs
    // .WithGPUSupport(OllamaGpuVendor.Nvidia); // uncomment on a machine with Nvidia drivers + nvidia-container-toolkit

// The model is a first-class resource: Aspire pulls it at startup.
var chat = ollama.AddModel("chat", "llama3.2"); // resource name "chat", Ollama model tag "llama3.2"

// The Blazor app gets the connection to the model and waits until it is ready.
builder.AddProject<Projects.Web>("web")
    .WithReference(chat)
    .WaitFor(chat);

builder.Build().Run();
