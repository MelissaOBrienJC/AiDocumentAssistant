using System.Net;
using System.Text;
using AiDocumentAssistant.Api.Services;

namespace AiDocumentAssistant.Tests;

public class DocumentEmbeddingServiceTests
{
    [Fact]
    public async Task GenerateEmbeddingAsync_ReturnsEmbedding_WhenResponseIsSuccessful()
    {
        // Arrange
        var embeddingValues = string.Join(",", Enumerable.Repeat("0.25", 768));

        var json = $$"""
        {
            "embeddings": [
                [{{embeddingValues}}]
            ]
        }
        """;

        var handler = new FakeHttpMessageHandler(json);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:11434/")
        };

        var service = new DocumentEmbeddingService(httpClient);

        // Act
        var result = await service.GenerateEmbeddingAsync(
            "The library opens at 9 AM.");

        // Assert
        Assert.Equal(768, result.Length);
        Assert.All(result, value => Assert.Equal(0.25f, value));
    }

    private class FakeHttpMessageHandler(string responseJson)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    responseJson,
                    Encoding.UTF8,
                    "application/json")
            });
        }
    }
}