namespace AiDocumentAssistant.Api.Services;

public class DocumentTextService
{
    public const int MaxTextLength = 4000;

    public string PrepareText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Document text cannot be empty.",
                nameof(text));
        }

        var preparedText = text.Trim();

        if (preparedText.Length > MaxTextLength)
        {
            throw new ArgumentException(
                $"Document text cannot exceed {MaxTextLength} characters.",
                nameof(text));
        }

        return preparedText;
    }
}