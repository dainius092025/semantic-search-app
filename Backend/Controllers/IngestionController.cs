using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Services;

namespace Backend.Controllers;


[ApiController]
[Route("api/[controller]")]
public class IngestionController : ControllerBase
{
    private readonly IngestionService _ingestionService;

    public IngestionController(IngestionService ingestionService)
    {
        _ingestionService = ingestionService;
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
            return StatusCode(500, $"Ingestion failed: {ex.Message}");
        }
    }
}


