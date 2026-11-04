namespace AiDocumentAssistant.Api.Services;

public class DocumentSemanticSearchService
{
    private readonly DocumentChunkingService _chunkingService;
    private readonly DocumentEmbeddingService _embeddingService;
    private readonly CosineSimilarityService _similarityService;

    public DocumentSemanticSearchService(
        DocumentChunkingService chunkingService,
        DocumentEmbeddingService embeddingService,
        CosineSimilarityService similarityService)
    {
        _chunkingService = chunkingService;
        _embeddingService = embeddingService;
        _similarityService = similarityService;
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

        var questionEmbedding =
            await _embeddingService.GenerateEmbeddingAsync(
                question,
                cancellationToken);

        var scoredChunks = new List<(string Chunk, double Score)>();

        foreach (var chunk in chunks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var chunkEmbedding =
                await _embeddingService.GenerateEmbeddingAsync(
                    chunk,
                    cancellationToken);

            var score = _similarityService.Calculate(
                questionEmbedding,
                chunkEmbedding);

            scoredChunks.Add((chunk, score));
        }

        return scoredChunks
            .OrderByDescending(item => item.Score)
            .Take(numberOfResults)
            .Select(item => item.Chunk)
            .ToList();
    }
}