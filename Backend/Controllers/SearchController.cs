using Microsoft.AspNetCore.Mvc;
using Backend.Services.Interfaces;
using Backend.Models.DTOs;
using Backend.Models;

namespace Backend.Controllers;


[ApiController] // marking this class as an api controller, which means it will handle HTTP requests and return JSON responses.

[Route ("api/[controller]")]//sets the base route for all endpoints in this controller, for example: /api/search

// this defines the SearchController class, which will handle all search related endpoints, such as POST /api/search
public class SearchController : ControllerBase
{
    
    private readonly IOllamaService _ollamaService;//Dependancy: service that communicats with the Ollama API to perform semantic search and get summaries. we will use it to convert the users search text into an embedding vector

    private readonly IStoryRepository _storyRepository;//Dependancy: repository that gives access to the story data. We will use it to search stories using the embedding vectors we get from the OllamaService

    //this is constructor, ASP.NET will automatically inject the dependencies (OllamaService and StoryRepository). this means we do not manually creaet these objects
    public SearchController(IOllamaService ollamaService, IStoryRepository storyRepository)
    {
        //store the injected Ollama service in a private variable
        _ollamaService = ollamaService;

        _storyRepository = storyRepository;
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

        Console.WriteLine("=== Search endpoint was hit ===");
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
        {   //convert the search text into an embedding vector using the Ollama service
            var embedding = await _ollamaService.GenerateEmbeddingAsync(request.Query);

            //search for stories in the repository that are similar to the embedding vector we got from the Ollama service
            var storyResults = await _storyRepository.SearchAsync(embedding, request.Limit);

         // // Map repository results to SearchResultDTO objects before returning them to the client
            var results = storyResults.Select(result => new SearchResultDTO
            {
                Id = result.Story.Id,
                Title = result.Story.Title,
                Author = result.Story.Author,
                Year = result.Story.Year,
                Summary = result.Story.Summary,
                // The similarity score (e.g., 0.95) is now provided by the repository.
                Similarity = result.Similarity
            }).ToList();

            return Ok(results);
        }
        catch (Exception ex)
        {   //if something fails, return HTTP 500
            Console.WriteLine("=== ERROR ===");
            Console.WriteLine(ex.ToString());

            return StatusCode(500, ex.Message);
        }
    }

    //POST /api/search/keyword endpoint does a keyword based search instead of semantic.
    //ok, again this bit tells to ASP.NET about Post request
    [HttpPost("keyword")]
    public async Task<IActionResult> KeywordSearch([FromBody] SearchRequestDTO request)
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
            //calls repository method and sends keyword query to DB and returns a list of mathcing story objects
            var stories = await _storyRepository.SearchByKeywordAsync(request.Query);

            //here happens mapping to dto
            var results = stories
                .Take(request.Limit)//only return the number of results requested
                .Select(Story => new SearchResultDTO//convert each story into dto
                {
                    Id = Story.Id,
                    Title = Story.Title,
                    Author = Story.Author,
                    Year = Story.Year,
                    Content = Story.Content,
                    Similarity = 0
                })
                .ToList();//executes transformation and converts results to a list
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
