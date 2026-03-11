using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Backend.Services;

namespace Backend.Controllers;


[ApiController]
[Route("api/[controller]")]
public class IngestionController : ControllerBase
{
    private readonly IngestionService _ingestionService;
    private readonly ILogger<IngestionController> _logger;

    public IngestionController(IngestionService ingestionService, ILogger<IngestionController> logger)
    {
        _ingestionService = ingestionService;
        _logger = logger;
    }

    [HttpPost("run")]
    public async Task<IActionResult> StartIngestion()
    {
        try 
        {
            await _ingestionService.RunFullIngestionAsync();
            return Ok("Ingestion completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during the ingestion process.");
            return StatusCode(500, $"Ingestion failed: {ex.Message}");
        }
    }
}
