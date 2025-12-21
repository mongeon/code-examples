namespace LocalRag.Core.Models;

public record EmbedRequest(string Input, string Model);
public record EmbedResponse(List<EmbedData> Data);
public record EmbedData(float[] Embedding);