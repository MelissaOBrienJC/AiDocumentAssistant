using System.Net;
using System.Text;
using AiDocumentAssistant.Api.Services;

namespace AiDocumentAssistant.Tests;

public class DocumentSemanticSearchServiceTests
{
    [Fact]
    public async Task FindRelevantChunksAsync_ReturnsChunks_WhenDocumentIsValid()
    {
        // Arrange
        var handler = new FakeEmbeddingHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:11434/")
        };

        var embeddingService = new DocumentEmbeddingService(httpClient);

        var service = new DocumentSemanticSearchService(
            new DocumentChunkingService(),
            embeddingService,
            new CosineSimilarityService());

        var document = "The library opens at 9 AM.";

        // Act
        var result = await service.FindRelevantChunksAsync(
            document,
            "When does the library open?");

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains("The library opens at 9 AM.", result[0]);
    }

    [Fact]
    public async Task FindRelevantChunksAsync_ThrowsException_WhenDocumentIsEmpty()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.FindRelevantChunksAsync(
                "",
                "When does the library open?"));
    }

    [Fact]
    public async Task FindRelevantChunksAsync_ThrowsException_WhenQuestionIsEmpty()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.FindRelevantChunksAsync(
                "The library opens at 9 AM.",
                ""));
    }

    [Fact]
    public async Task FindRelevantChunksAsync_ThrowsException_WhenNumberOfResultsIsInvalid()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.FindRelevantChunksAsync(
                "The library opens at 9 AM.",
                "When does the library open?",
                numberOfResults: 0));
    }

    private static DocumentSemanticSearchService CreateService()
    {
        var handler = new FakeEmbeddingHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:11434/")
        };

        return new DocumentSemanticSearchService(
            new DocumentChunkingService(),
            new DocumentEmbeddingService(httpClient),
            new CosineSimilarityService());
    }

    private class FakeEmbeddingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var embedding = string.Join(
                ",",
                Enumerable.Repeat("0.25", 768));

            var json = $$"""
            {
                "embeddings": [
                    [{{embedding}}]
                ]
            }
            """;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json")
            });
        }
    }
}