
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AiDocumentAssistant.Api.Services;

public class DocumentSummaryService
{
    private readonly HttpClient _httpClient;

    public DocumentSummaryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> SummarizeAsync(
        string documentText,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            model = "qwen2.5:0.5b",
            prompt =
                "You are a document summarization assistant.\n\n" +
                "Summarize the document using ONLY information explicitly " +
                "stated in the document.\n\n" +
                "IMPORTANT RULES:\n" +
                "1. Never invent or assume facts, dates, names, or numbers.\n" +
                "2. Do not add information from your own knowledge.\n" +
                "3. If a date or detail is not provided, do not guess it.\n" +
                "4. Preserve the meaning of the original document.\n" +
                "5. Focus on the main points and important conclusions.\n" +
                "6. Use clear, concise language.\n\n" +
                "DOCUMENT:\n" +
                documentText,
            stream = false,
            options = new
            {
                num_gpu = 0,
                num_predict = 200,
                temperature = 0
            }
        };

        using var response = await _httpClient.PostAsJsonAsync(
            "api/generate",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(
            cancellationToken: cancellationToken);

        if (string.IsNullOrWhiteSpace(result?.Response))
        {
            throw new InvalidOperationException(
                "The AI model returned an empty summary.");
        }

        return result.Response.Trim();
    }

    private sealed class OllamaResponse
    {
        [JsonPropertyName("response")]
        public string? Response { get; set; }
    }
}
