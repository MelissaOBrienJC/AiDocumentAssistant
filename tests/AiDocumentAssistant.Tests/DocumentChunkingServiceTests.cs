using AiDocumentAssistant.Api.Services;

namespace AiDocumentAssistant.Tests;

public class DocumentChunkingServiceTests
{
    private readonly DocumentChunkingService _service = new();

    [Fact]
    public void SplitIntoChunks_ReturnsSingleChunk_WhenTextIsShort()
    {
        // Arrange
        var text = "The library opens at 9 AM.";

        // Act
        var result = _service.SplitIntoChunks(text);

        // Assert
        Assert.Single(result);
        Assert.Equal(text, result[0]);
    }

    [Fact]
    public void SplitIntoChunks_CreatesMultipleChunks_WhenTextIsLong()
    {
        // Arrange
        var text = new string('A', 1200);

        // Act
        var result = _service.SplitIntoChunks(text);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(500, result[0].Length);
        Assert.Equal(500, result[1].Length);
        Assert.Equal(300, result[2].Length);
    }

    [Fact]
    public void SplitIntoChunks_OverlapsAdjacentChunks()
    {
        // Arrange
        var text = new string('A', 600);

        // Act
        var result = _service.SplitIntoChunks(text);

        // Assert
        Assert.Equal(
            result[0][450..],
            result[1][..50]);
    }

    [Fact]
    public void SplitIntoChunks_ThrowsException_WhenTextIsEmpty()
    {
        Assert.Throws<ArgumentException>(
            () => _service.SplitIntoChunks(""));
    }

    [Fact]
    public void SplitIntoChunks_ThrowsException_WhenOverlapIsInvalid()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _service.SplitIntoChunks(
                "Some text",
                chunkSize: 100,
                overlap: 100));
    }
}