
using AiDocumentAssistant.Api.Services;

namespace AiDocumentAssistant.Tests;

public class DocumentKeywordSearchServiceTests
{
    [Fact]
    public void CalculateScore_ReturnsOne_WhenAllKeywordsMatch()
    {
        var service = new DocumentKeywordSearchService();

        var result = service.CalculateScore(
            "The library opens at 9 AM on weekdays.",
            "When does the library open?");

        Assert.Equal(1.0, result);
    }

    [Fact]
    public void CalculateScore_ReturnsPartialScore_WhenSomeKeywordsMatch()
    {
        var service = new DocumentKeywordSearchService();

        var result = service.CalculateScore(
            "The library opens at 9 AM.",
            "When does the library open on Saturdays?");

        Assert.Equal(2.0 / 3.0, result);
    }

    [Fact]
    public void CalculateScore_ReturnsZero_WhenNoKeywordsMatch()
    {
        var service = new DocumentKeywordSearchService();

        var result = service.CalculateScore(
            "The library opens at 9 AM.",
            "What is the population of Chicago?");

        Assert.Equal(0.0, result);
    }

    [Fact]
    public void CalculateScore_ReturnsZero_WhenQuestionIsEmpty()
    {
        var service = new DocumentKeywordSearchService();

        var result = service.CalculateScore(
            "The library opens at 9 AM.",
            "");

        Assert.Equal(0.0, result);
    }
}
