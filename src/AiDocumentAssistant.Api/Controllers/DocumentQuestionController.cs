
using AiDocumentAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiDocumentAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentQuestionController : ControllerBase
{
    private readonly DocumentQuestionService _questionService;
    private readonly DocumentTextService _textService;
    private readonly DocumentSemanticSearchService _semanticSearchService;

    public DocumentQuestionController(
        DocumentQuestionService questionService,
        DocumentTextService textService,
        DocumentSemanticSearchService semanticSearchService)
    {
        _questionService = questionService;
        _textService = textService;
        _semanticSearchService = semanticSearchService;
    }

    public record AskQuestionRequest(string? Text, string? Question);

    [HttpPost]
    public async Task<ActionResult<object>> Ask(
        [FromBody] AskQuestionRequest request,
        CancellationToken cancellationToken)
    {
        string preparedText;

        try
        {
            preparedText = _textService.PrepareText(request.Text);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest(new
            {
                error = "Please enter a question."
            });
        }

        if (request.Question.Length > 500)
        {
            return BadRequest(new
            {
                error = "The question cannot exceed 500 characters."
            });
        }

        try
        {
            var relevantChunks =
                await _semanticSearchService.FindRelevantChunksAsync(
                    preparedText,
                    request.Question.Trim(),
                    cancellationToken: cancellationToken);

            if (relevantChunks.Count == 0)
            {
                return Ok(new
                {
                    answer = "The document does not provide that information."
                });
            }

            var evidenceText = string.Join("\n\n", relevantChunks);

            var answer = await _questionService.AnswerAsync(
                evidenceText,
                request.Question.Trim(),
                cancellationToken);

            return Ok(new { answer });
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            return StatusCode(502, new
            {
                error = "The AI question-answering service failed. Please try again."
            });
        }
    }
}
