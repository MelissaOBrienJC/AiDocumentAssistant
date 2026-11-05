
using System.Text.RegularExpressions;

namespace AiDocumentAssistant.Api.Services;

public class DocumentKeywordSearchService
{
    private static readonly HashSet<string> StopWords =
    [
        "a", "an", "and", "are", "at", "does", "do",
        "for", "from", "how", "is", "it", "of", "on",
        "or", "the", "to", "was", "when", "what",
        "where", "which", "who", "why", "with", "this"
    ];

    public double CalculateScore(string chunk, string question)
    {
        if (string.IsNullOrWhiteSpace(chunk) ||
            string.IsNullOrWhiteSpace(question))
        {
            return 0;
        }

        var keywords = Regex
            .Matches(question.ToLowerInvariant(), @"\b[a-z0-9]+\b")
            .Select(match => match.Value)
            .Where(word => !StopWords.Contains(word))
            .Select(NormalizeWord)
            .Where(word => word.Length > 2)
            .Distinct()
            .ToList();

        if (keywords.Count == 0)
        {
            return 0;
        }

        var chunkWords = Regex
            .Matches(chunk.ToLowerInvariant(), @"\b[a-z0-9]+\b")
            .Select(match => NormalizeWord(match.Value))
            .ToHashSet();

        var matches = keywords.Count(
            keyword => chunkWords.Contains(keyword));

        return (double)matches / keywords.Count;
    }

    private static string NormalizeWord(string word)
    {
        // Basic normalization: "opens" becomes "open".
        if (word.Length > 3 && word.EndsWith('s'))
        {
            return word[..^1];
        }

        return word;
    }
}
