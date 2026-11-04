
using System.Net;
using System.Text;
using System.Text.Json;
using AiDocumentAssistant.Api.Services;

namespace AiDocumentAssistant.Tests;

public class DocumentSummaryServiceTests
{
    [Fact]
    public async Task SummarizeAsync_ReturnsSummaryFromOllama()
    {
        // Arrange: simulate a successful Ollama response.
        var handler = new FakeHttpMessageHandler(
            """{"response":"The library will extend weekday hours."}""");

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:11434/")
        };

        var service = new DocumentSummaryService(httpClient);

        // Act: ask the service to summarize a document.
        var summary = await service.SummarizeAsync(
            "The library will close at 8 PM on weekdays.");

        // Assert: verify the summary and request settings.
        Assert.Equal(
            "The library will extend weekday hours.",
            summary);

        using var requestJson = JsonDocument.Parse(handler.RequestBody!);
        var root = requestJson.RootElement;

        Assert.Equal("qwen2.5:0.5b", root.GetProperty("model").GetString());
        Assert.False(root.GetProperty("stream").GetBoolean());
        Assert.Equal(
            0,
            root.GetProperty("options").GetProperty("num_gpu").GetInt32());
    }

    [Fact]
    public async Task SummarizeAsync_WhenOllamaFails_ThrowsHttpRequestException()
    {
        // Arrange: simulate Ollama returning HTTP 500.
        var handler = new FakeHttpMessageHandler(
            """{"error":"model unavailable"}""",
            HttpStatusCode.InternalServerError);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:11434/")
        };

        var service = new DocumentSummaryService(httpClient);

        // Act and assert: the service should report the HTTP failure.
        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.SummarizeAsync("Test document"));
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseBody;
        private readonly HttpStatusCode _statusCode;

        public string? RequestBody { get; private set; }

        public FakeHttpMessageHandler(
            string responseBody,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _responseBody = responseBody;
            _statusCode = statusCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestBody = await request.Content!
                .ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(
                    _responseBody,
                    Encoding.UTF8,
                    "application/json")
            };
        }
    }
}
