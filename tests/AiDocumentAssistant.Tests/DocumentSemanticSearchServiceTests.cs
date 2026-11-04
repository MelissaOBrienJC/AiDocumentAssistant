
using System.Net;
using System.Text;
using AiDocumentAssistant.Api.Services;
using Microsoft.Extensions.Caching.Memory;

namespace AiDocumentAssistant.Tests;

public class DocumentSemanticSearchServiceTests
{
    [Fact]
    public async Task FindRelevantChunksAsync_ReturnsChunks_WhenDocumentIsValid()
    {
        // Arrange
        var handler = new FakeEmbeddingHandler();
        var service = CreateService(handler);

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
    public async Task FindRelevantChunksAsync_ReusesCachedDocumentEmbeddings()
    {
        // Arrange
        var handler = new FakeEmbeddingHandler();
        var service = CreateService(handler);

        var document = "The library opens at 9 AM.";

        // Act
        await service.FindRelevantChunksAsync(
            document,
            "When does the library open?");

        await service.FindRelevantChunksAsync(
            document,
            "What time does the library start operating?");

        // Assert
        // Two question embeddings + one document chunk embedding.
        Assert.Equal(3, handler.RequestCount);
    }

    [Fact]
    public async Task FindRelevantChunksAsync_ThrowsException_WhenDocumentIsEmpty()
    {
        var service = CreateService(new FakeEmbeddingHandler());

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.FindRelevantChunksAsync(
                "",
                "When does the library open?"));
    }

    [Fact]
    public async Task FindRelevantChunksAsync_ThrowsException_WhenQuestionIsEmpty()
    {
        var service = CreateService(new FakeEmbeddingHandler());

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.FindRelevantChunksAsync(
                "The library opens at 9 AM.",
                ""));
    }

    [Fact]
    public async Task FindRelevantChunksAsync_ThrowsException_WhenNumberOfResultsIsInvalid()
    {
        var service = CreateService(new FakeEmbeddingHandler());

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.FindRelevantChunksAsync(
                "The library opens at 9 AM.",
                "When does the library open?",
                numberOfResults: 0));
    }

    private static DocumentSemanticSearchService CreateService(
        FakeEmbeddingHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:11434/")
        };

        return new DocumentSemanticSearchService(
            new DocumentChunkingService(),
            new DocumentEmbeddingService(httpClient),
            new CosineSimilarityService(),
            new MemoryCache(new MemoryCacheOptions()));
    }

    private class FakeEmbeddingHandler : HttpMessageHandler
    {
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestCount++;

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
