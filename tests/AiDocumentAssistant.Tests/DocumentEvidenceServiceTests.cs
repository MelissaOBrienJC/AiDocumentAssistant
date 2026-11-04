
using AiDocumentAssistant.Api.Services;

namespace AiDocumentAssistant.Tests;

public class DocumentEvidenceServiceTests
{
    private readonly DocumentEvidenceService _service = new();

    [Fact]
    public void FindRelevantSentences_ReturnsMatchingSentence()
    {
        // Arrange
        var document =
            "The library will extend weekday hours until 8 PM starting November 1. " +
            "Saturday hours will remain 10 AM to 4 PM.";

        var question = "When will weekday hours be extended?";

        // Act
        var result = _service.FindRelevantSentences(
            document,
            question);

        // Assert
        Assert.Contains(
            result,
            sentence => sentence.Contains("weekday hours"));
    }

    [Fact]
    public void FindRelevantSentences_ReturnsEmptyWhenNoMatch()
    {
        // Arrange
        var document =
            "The library will extend weekday hours until 8 PM starting November 1.";

        var question = "What is the library's parking policy?";

        // Act
        var result = _service.FindRelevantSentences(
            document,
            question);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindRelevantSentences_ReturnsEmptyForEmptyQuestion()
    {
        // Arrange
        var document = "The library opens at 9 AM.";

        // Act
        var result = _service.FindRelevantSentences(
            document,
            "");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FindRelevantSentences_IgnoresCommonQuestionWords()
    {
        // Arrange
        var document =
            "The library will extend weekday hours until 8 PM.";

        var question = "What will the library do?";

        // Act
        var result = _service.FindRelevantSentences(
            document,
            question);

        // Assert
        Assert.Empty(result);
    }
}
