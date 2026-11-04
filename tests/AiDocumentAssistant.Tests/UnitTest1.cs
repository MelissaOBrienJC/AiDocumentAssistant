
using AiDocumentAssistant.Api.Services;

namespace AiDocumentAssistant.Tests;

public class DocumentTextServiceTests
{
    private readonly DocumentTextService _service = new();

    [Fact]
    public void PrepareText_TrimsLeadingAndTrailingWhitespace()
    {
        var result = _service.PrepareText("  Hello world  ");

        Assert.Equal("Hello world", result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\r\n\t")]
    public void PrepareText_RejectsEmptyText(string? text)
    {
        Assert.Throws<ArgumentException>(
            () => _service.PrepareText(text));
    }

    [Fact]
    public void PrepareText_RejectsTextOverMaximumLength()
    {
        var text = new string('a',
            DocumentTextService.MaxTextLength + 1);

        Assert.Throws<ArgumentException>(
            () => _service.PrepareText(text));
    }

    [Fact]
    public void PrepareText_AcceptsTextAtMaximumLength()
    {
        var text = new string('a',
            DocumentTextService.MaxTextLength);

        var result = _service.PrepareText(text);

        Assert.Equal(text, result);
    }
}
