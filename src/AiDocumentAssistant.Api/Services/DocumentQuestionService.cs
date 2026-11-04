
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
          model = "qwen2.5:3b",
            prompt =
                "You answer questions about a document.\n\n" +
                "RULES:\n" +
                "- Base your answer ONLY on information in the document.\n" +
                "- Understand questions expressed using different words or phrases " +
                "that have the same meaning as information in the document.\n" +
                "- You may make reasonable interpretations of the document's " +
                "wording, but do not introduce unsupported facts.\n" +
                "- Do not use outside knowledge or make assumptions about " +
                "information the document does not contain.\n" +
                "- If the document does not contain enough information to answer, " +
                "respond with exactly: " +
                "\"The document does not provide that information.\"\n" +
                "- Keep answers concise and direct.\n\n" +
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
