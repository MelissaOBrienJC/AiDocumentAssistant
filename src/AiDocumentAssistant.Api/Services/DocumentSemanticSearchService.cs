
using Microsoft.Extensions.Caching.Memory;

namespace AiDocumentAssistant.Api.Services;

public class DocumentSemanticSearchService
{
    private readonly DocumentChunkingService _chunkingService;
    private readonly DocumentEmbeddingService _embeddingService;
    private readonly CosineSimilarityService _similarityService;
    private readonly IMemoryCache _cache;

    public DocumentSemanticSearchService(
        DocumentChunkingService chunkingService,
        DocumentEmbeddingService embeddingService,
        CosineSimilarityService similarityService,
        IMemoryCache cache)
    {
        _chunkingService = chunkingService;
        _embeddingService = embeddingService;
        _similarityService = similarityService;
        _cache = cache;
    }

    public async Task<List<string>> FindRelevantChunksAsync(
        string documentText,
        string question,
        int numberOfResults = 3,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(documentText))
        {
            throw new ArgumentException(
                "Document text cannot be empty.",
                nameof(documentText));
        }

        if (string.IsNullOrWhiteSpace(question))
        {
            throw new ArgumentException(
                "Question cannot be empty.",
                nameof(question));
        }

        if (numberOfResults <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(numberOfResults));
        }

        var chunks = _chunkingService.SplitIntoChunks(documentText);

        // Generate an embedding for the question.
        var questionEmbedding =
            await _embeddingService.GenerateEmbeddingAsync(
                question,
                cancellationToken);

        var scoredChunks = new List<(string Chunk, double Score)>();

        foreach (var chunk in chunks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Reuse the cached embedding if we have already processed this chunk.
            var cacheKey = $"document-embedding:nomic-embed-text:{chunk}";

            if (!_cache.TryGetValue(
                cacheKey,
                out float[]? chunkEmbedding))
            {
                chunkEmbedding =
                    await _embeddingService.GenerateEmbeddingAsync(
                        chunk,
                        cancellationToken);

                _cache.Set(
                    cacheKey,
                    chunkEmbedding,
                    TimeSpan.FromHours(1));
            }

            var score = _similarityService.Calculate(
                questionEmbedding,
                chunkEmbedding!);

            scoredChunks.Add((chunk, score));
        }

        return scoredChunks
            .OrderByDescending(item => item.Score)
            .Take(numberOfResults)
            .Select(item => item.Chunk)
            .ToList();
    }
}
