using LocalRag.Core.Utils;

namespace LocalRag.Core.Services;

public class Indexer(EmbeddingClient embeddingClient, QdrantClient qdrantClient)
{
    public async Task IndexDocumentAsync(string docId, string text)
    {
        var chunks = Ingest.Chunk(text).ToList();
        Console.WriteLine($"📄 Document '{docId}' split into {chunks.Count} chunks");

        int chunkIndex = 0;
        foreach (var chunk in chunks)
        {
            try
            {
                var embedding = await embeddingClient.EmbedAsync(chunk);

                var payload = new Dictionary<string, object>
                {
                    ["content"] = chunk,
                    ["docId"] = docId,
                    ["chunkIndex"] = chunkIndex
                };

                var id = Guid.NewGuid();
                await qdrantClient.UpsertAsync(id, embedding, payload);
                chunkIndex++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error indexing chunk {chunkIndex}: {ex.Message}");
            }
        }

        Console.WriteLine($"✅ Indexed {chunkIndex} chunks for '{docId}'\n");
    }
}