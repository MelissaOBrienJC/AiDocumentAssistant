
using AiDocumentAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiDocumentAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentTextController : ControllerBase
{
    private readonly DocumentTextService _service;

    public DocumentTextController(DocumentTextService service)
    {
        _service = service;
    }

    public record PrepareTextRequest(string? Text);

    [HttpPost("prepare")]
    public ActionResult<string> Prepare(
        [FromBody] PrepareTextRequest request)
    {
        try
        {
            var result = _service.PrepareText(request.Text);
            return Ok(new { text = result });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
