
using AiDocumentAssistant.Api.Controllers;
using AiDocumentAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace AiDocumentAssistant.Tests;

public class DocumentSummaryControllerTests
{
    [Fact]
    public async Task Summarize_WhenAiServiceFails_Returns502()
    {
        // Arrange: use an HttpClient that simulates an Ollama failure.
        var handler = new FailingHttpMessageHandler();

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:11434/")
        };

        var summaryService = new DocumentSummaryService(httpClient);
        var textService = new DocumentTextService();

        var controller = new DocumentSummaryController(
            summaryService,
            textService);

        var request = new DocumentSummaryController.SummarizeRequest(
            "This is a test document.");

        // Act: invoke the controller.
        var result = await controller.Summarize(
            request,
            CancellationToken.None);

        // Assert: the controller should return HTTP 502.
        var statusResult = Assert.IsType<ObjectResult>(result.Result);

        Assert.Equal(502, statusResult.StatusCode);

        var json = JsonSerializer.Serialize(statusResult.Value);
        using var document = JsonDocument.Parse(json);

        Assert.Equal(
            "The AI summarization service failed. Please try again.",
            document.RootElement.GetProperty("error").GetString());
            }

[Fact]
public async Task Summarize_WhenTextIsEmpty_Returns400()
{
    var handler = new FailingHttpMessageHandler();

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:11434/")
    };

    var summaryService = new DocumentSummaryService(httpClient);
    var textService = new DocumentTextService();

    var controller = new DocumentSummaryController(
        summaryService,
        textService);

    var request = new DocumentSummaryController.SummarizeRequest("");

    var result = await controller.Summarize(
        request,
        CancellationToken.None);

    var badRequest = Assert.IsType<BadRequestObjectResult>(
        result.Result);

    Assert.Equal(400, badRequest.StatusCode);
}
    private sealed class FailingHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(
                HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(
                    """{"error":"model unavailable"}""")
            });
        }
    }

    private sealed class ErrorResponse
    {
        public string? error { get; set; }
    }
}
