
using AiDocumentAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiDocumentAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentSummaryController : ControllerBase
{
    private readonly DocumentSummaryService _service;
    private readonly DocumentTextService _textService;

    public DocumentSummaryController(
        DocumentSummaryService service,
        DocumentTextService textService)
    {
        _service = service;
        _textService = textService;
    }

    public record SummarizeRequest(string? Text);

    [HttpPost]
    public async Task<ActionResult<object>> Summarize(
        [FromBody] SummarizeRequest request,
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

        try
        {
            var summary = await _service.SummarizeAsync(
                preparedText,
                cancellationToken);

            return Ok(new { summary });
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
                error = "The AI summarization service failed. Please try again."
            });
        }
    }
}
