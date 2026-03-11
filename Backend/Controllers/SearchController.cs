using Microsoft.AspNetCore.Mvc;
using Backend.Services.Interfaces;
using Backend.Models.DTOs;

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
        //checkif the query is empty or null, if it is return http 400 bad request with a message.
        if (string.IsNullOrWhiteSpace(request.Query))
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
            var stories = await _storyRepository.SearchAsync(embedding);

            // Convert the story objects into DTOs that are safe to return to the frontend
            var results = stories.Take(request.Limit).Select(story => new SearchResultDTO
            {
                Id = story.Id,
                Title = story.Title,
                Author = story.Author,
                Year = story.Year,
                Summary = story.Summary,
                Similarity = 0
            }).ToList();

            return Ok(results);
        }
        catch (Exception)
        {   //if something fails, return HTTP 500
            return StatusCode(500, "An unexpected error occured.");
        }
    }



}


/* 
backend search flow
User types query
      ↓
POST /api/search
      ↓
SearchController
      ↓
OllamaService → convert text → embedding vector
      ↓
StoryRepository → search stories by vector similarity
      ↓
Return SearchResultDTO list 
*/