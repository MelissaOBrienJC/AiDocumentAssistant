
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AiDocumentAssistant.Api.Services;

public class DocumentEmbeddingService
{
    private readonly HttpClient _httpClient;

    public DocumentEmbeddingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<float[]> GenerateEmbeddingAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Text cannot be empty.",
                nameof(text));
        }

        var request = new
        {
            model = "nomic-embed-text",
            input = text
        };

        using var response = await _httpClient.PostAsJsonAsync(
            "api/embed",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>(
            cancellationToken: cancellationToken);

        var embedding = result?.Embeddings?.FirstOrDefault();

        if (embedding == null || embedding.Length == 0)
        {
            throw new InvalidOperationException(
                "Ollama returned an empty embedding.");
        }

        return embedding;
    }

    private sealed class OllamaEmbeddingResponse
    {
        [JsonPropertyName("embeddings")]
        public float[][]? Embeddings { get; set; }
    }
}
