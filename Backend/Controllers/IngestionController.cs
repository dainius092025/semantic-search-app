using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            Console.WriteLine(ex);
            return StatusCode(500, "Ingestion failed due to an internal server error. - the developer");
        }
    }

    // GET /api/ingestion/status
    // Returns whether all stories are fully ingested or not.
    // This is a read-only endpoint that does not modify any data.
    [HttpGet("status")]
    public async Task<IActionResult> GetIngestionStatus()
    {
        try
        {
            // Ask the service whether all stories are fully ingested
            var isComplete = await _ingestionService.IsIngestionCompleteAsync();

            // Return a clear status response
            return Ok(new
            {
                IsComplete = isComplete,
                Message = isComplete
                    ? "All stories are fully ingested."
                    : "Some stories are missing embeddings or summaries."
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "An internal server error occurred. - the developer");
        }
    }
}


