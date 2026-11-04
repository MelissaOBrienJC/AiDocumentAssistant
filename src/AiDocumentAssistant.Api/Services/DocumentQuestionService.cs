
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AiDocumentAssistant.Api.Services;

public class DocumentQuestionService
{
    private readonly HttpClient _httpClient;

    public DocumentQuestionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> AnswerAsync(
        string documentText,
        string question,
        CancellationToken cancellationToken = default)
    {
        var request = new
        {
            model = "qwen2.5:0.5b",
            prompt =
                "You answer questions about a document.\n\n" +
                "STRICT RULES:\n" +
                "- Use ONLY facts explicitly written in the document.\n" +
                "- Never guess, infer, calculate, or assume an answer.\n" +
                "- Similar information is NOT sufficient evidence.\n" +
                "- If the exact answer is not stated, respond with exactly: " +
                "\"The document does not provide that information.\"\n" +
                "- Do not use general knowledge.\n\n" +
                "DOCUMENT:\n" +
                documentText +
                "\n\nQUESTION:\n" +
                question +
                "\n\nANSWER:",
            stream = false,
            options = new
            {
                num_gpu = 0,
                num_predict = 100,
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
                "The AI model returned an empty answer.");
        }

        return result.Response.Trim();
    }

    private sealed class OllamaResponse
    {
        [JsonPropertyName("response")]
        public string? Response { get; set; }
    }
}
