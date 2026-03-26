using Microsoft.AspNetCore.Mvc;
using Backend.Services.Interfaces;
using Backend.Models.DTOs;
using Backend.Models;

namespace Backend.Controllers;

// Delegates all search logic to the service layer.
// Keeps the controller focused on HTTP request/response handling.


[ApiController] // marking this class as an api controller, which means it will handle HTTP requests and return JSON responses.

[Route ("api/[controller]")]//sets the base route for all endpoints in this controller, for example: /api/search

// this defines the SearchController class, which will handle all search related endpoints, such as POST /api/search
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    
  
    // Inject SearchService instead of multiple dependencies. This moves all search logic (semantic + keyword + ranking) into the service layer, keeping the controller simple and focused only on handling HTTP requests.
    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    //this attribute marks this method as an HTTP POST endpoint, the final route becomes: POST /api/search
    [HttpPost]

    //this method handles search request ftom the frontend.
            //async method runs asynchronously, non blocking
                //Task represents an asynchronous operation
                    //IActionResult is flexible HTTP response(ok, BadRequest or others)
                                            //fromBody tells ASP.NET to read JSON from the request body and convert it into a SearchRequestDTO object automaticcaly.
    public async Task<IActionResult> Search([FromBody] SearchRequestDTO request)
    {
        //checkif the query is empty or null, if it is return http 400 bad request with a message.
        if (request == null || string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest("Query cannot be empty");
        }
        //check if the query is too short, if it is return http 400 bad request with a message.
        if (request.Query.Trim().Length < 3)
        {
            return BadRequest("Quest must be at least 3 characters long");
        }
        // Ensure limit stays within a safe range, so that users(frontend) cannot request too many results at once, which could overload the system. If the limit is less than or equal to 0, or greater than 20, we default it to 5.
        if (request.Limit <= 0 || request.Limit > 20)
        {
            request.Limit = 5;
        }

        try
        {
            //******NEW******
            //all search-related logic (semantic, keyword, and hybrid ranking) is encapsulated inside SearchService. The controller simply delegates the request, resulting in cleaner, more maintainable, and testable code.
            var results = await _searchService.SemanticSearchWithKeywordBoostAsync(request);
            return Ok(results);
        }
        catch (Exception ex)
        {   //if something fails, return HTTP 500
            Console.WriteLine("=== ERROR ===");
            Console.WriteLine(ex.ToString());

            return StatusCode(500, ex.Message);
        }
    }

    //POST /api/search/metadata endpoint does a metadata based search instead of semantic.
    //ok, again this bit tells to ASP.NET about Post request
    [HttpPost("metadata")]
    public async Task<IActionResult> MetadataSearch([FromBody] SearchRequestDTO request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest("Query cannot be empty");
        }

        if (request.Query.Trim().Length < 3)
        {
            return BadRequest("Query must be at least 3 characters long");
        }

        if (request.Limit <= 0 || request.Limit > 20)
        {
            request.Limit = 5;
        }
        
        try
        {
            var results = await _searchService.MetadataSearchAsync(request);
            return Ok(results);
        }

        catch (Exception ex)
        {
            Console.WriteLine("KEYWORD SEARCH ERROR");
            Console.WriteLine(ex.ToString());

            return StatusCode(500, "an internal server error occured - the developer");
        }

    }
}
