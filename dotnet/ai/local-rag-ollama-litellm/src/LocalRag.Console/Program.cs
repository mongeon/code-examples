using LocalRag.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    // HttpClient for LiteLLM (Embedding + Chat)
    services.AddHttpClient<EmbeddingClient>(c =>
    {
        c.BaseAddress = new Uri("http://localhost:8080");
        c.Timeout = TimeSpan.FromSeconds(30);
    });

    // Qdrant Client (using gRPC SDK)
    services.AddSingleton<QdrantClient>(sp =>
        new QdrantClient("localhost", 6334, "docs"));

    // RAG Service (needs both embedding and qdrant, plus a general http client)
    services.AddHttpClient<RagService>(c =>
    {
        c.Timeout = TimeSpan.FromSeconds(60);
    });

    services.AddScoped<Indexer>();
});

var host = builder.Build();

// Run the RAG pipeline
using var scope = host.Services.CreateScope();
var indexer = scope.ServiceProvider.GetRequiredService<Indexer>();
var ragService = scope.ServiceProvider.GetRequiredService<RagService>();

// Create documents folder if it doesn't exist
var docFolder = "documents";
if (!Directory.Exists(docFolder))
{
    Directory.CreateDirectory(docFolder);
}

// Create sample documents for testing
var sampleFiles = new Dictionary<string, string>
{
    ["embeddings.md"] = @"Embeddings are dense vectors that represent the semantic meaning of text.
            Each embedding is typically 384-768 dimensions long.
            Similar concepts cluster together in vector space.
            Embeddings are created by machine learning models specialized in semantic encoding.
            They enable fast similarity search in vector databases.",

    ["rag.md"] = @"RAG stands for Retrieval-Augmented Generation.
            It combines information retrieval with language model generation.
            RAG improves accuracy by grounding responses in retrieved documents.
            The process has two phases: indexing (offline) and querying (online).
            RAG reduces hallucinations in large language models.",

    ["ollama.md"] = @"Ollama is a framework for running large language models locally.
            It supports models like Llama, Mistral, Qwen, and many others.
            Ollama can run on consumer hardware with reasonable performance.
            It provides an OpenAI-compatible API endpoint on port 11434.
            Ollama is completely free and open source."
};

Console.WriteLine("📚 Setting up sample documents...\n");
foreach (var (filename, content) in sampleFiles)
{
    var filepath = Path.Combine(docFolder, filename);
    if (!File.Exists(filepath))
    {
        await File.WriteAllTextAsync(filepath, content);
        Console.WriteLine($"✓ Created {filename}");
    }
}

Console.WriteLine("\n⏳ Indexing documents into Qdrant...\n");

// Index all documents
foreach (var file in Directory.GetFiles(docFolder, "*.md"))
{
    var docId = Path.GetFileNameWithoutExtension(file);
    var text = await File.ReadAllTextAsync(file);
    await indexer.IndexDocumentAsync(docId, text);
}

Console.WriteLine("✅ Indexing complete!\n");
Console.WriteLine("=".PadRight(60, '='));
Console.WriteLine("\n🚀 Starting RAG Query Tests\n");
Console.WriteLine("=".PadRight(60, '=') + "\n");

// Test queries
var testQueries = new[]
{
        "What are embeddings?",
        "How does RAG work?",
        "What is Ollama?",
        "How can I use embeddings for similarity search?"
    };

foreach (var query in testQueries)
{
    try
    {
        await ragService.AskAsync(query);
        Console.WriteLine("-".PadRight(60, '-') + "\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Query failed: {ex.Message}\n");
    }
}

Console.WriteLine("✅ All tests completed!");
