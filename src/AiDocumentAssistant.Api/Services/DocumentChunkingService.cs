namespace AiDocumentAssistant.Api.Services;

public class DocumentChunkingService
{
    public List<string> SplitIntoChunks(
        string text,
        int chunkSize = 500,
        int overlap = 50)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Document text cannot be empty.",
                nameof(text));
        }

        if (chunkSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkSize));
        }

        if (overlap < 0 || overlap >= chunkSize)
        {
            throw new ArgumentOutOfRangeException(nameof(overlap));
        }

        var chunks = new List<string>();

        for (var start = 0; start < text.Length;)
        {
            var length = Math.Min(chunkSize, text.Length - start);

            var chunk = text.Substring(start, length).Trim();

            if (!string.IsNullOrWhiteSpace(chunk))
            {
                chunks.Add(chunk);
            }

            if (start + length >= text.Length)
            {
                break;
            }

            start += chunkSize - overlap;
        }

        return chunks;
    }
}