using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.Services.Interfaces;

namespace Backend.Controllers;


[ApiController]
[Route("api/[controller]")]
public class IngestionController : ControllerBase
{
    private readonly IStoryIngestionService _ingestionService;

    public IngestionController(IStoryIngestionService ingestionService)
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
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            return StatusCode(500, $"Ingestion failed: {errorMessage}");
        }
    }
}


