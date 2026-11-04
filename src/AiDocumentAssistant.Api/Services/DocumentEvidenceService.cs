
using System.Text.RegularExpressions;

namespace AiDocumentAssistant.Api.Services;

public class DocumentEvidenceService
{
    public List<string> FindRelevantSentences(
        string documentText,
        string question)
    {
        if (string.IsNullOrWhiteSpace(documentText) ||
            string.IsNullOrWhiteSpace(question))
        {
            return [];
        }

        var questionWords = Regex
            .Matches(question.ToLowerInvariant(), @"\b[a-z0-9]+\b")
            .Select(match => match.Value)
            .Where(word => word.Length > 2)
            .Where(word => !StopWords.Contains(word))
            .Distinct()
            .ToHashSet();

        if (questionWords.Count == 0)
        {
            return [];
        }

        var sentences = Regex.Split(
            documentText,
            @"(?<=[.!?])\s+");

        var matches = new List<string>();

        foreach (var sentence in sentences)
        {
            var sentenceWords = Regex
                .Matches(sentence.ToLowerInvariant(), @"\b[a-z0-9]+\b")
                .Select(match => match.Value)
                .ToHashSet();

            var matchingWords = questionWords
                .Intersect(sentenceWords)
                .Count();

            if (matchingWords >= 2 ||
                (questionWords.Count == 1 && matchingWords == 1))
            {
                matches.Add(sentence.Trim());
            }
        }

        return matches;
    }

    private static readonly HashSet<string> StopWords =
    [
        "the", "and", "for", "does", "what", "when",
        "where", "which", "who", "how", "why",
        "was", "were", "are", "is", "will",
        "would", "could", "should", "can",
        "this", "that", "these", "those",
        "from", "with", "about", "have", "has",
        "had", "not", "into", "their", "there",
        "they", "them", "you", "your",
        "library", "document", "information", "provide",
"tell", "show", "describe", "do"
    ];
}
