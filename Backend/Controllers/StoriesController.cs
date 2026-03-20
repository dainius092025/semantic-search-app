using Microsoft.AspNetCore.Mvc;
using Backend.Services.Interfaces;
using Backend.Models.DTOs;
namespace Backend.Controllers;

/// <summary>
/// This file defines the StoriesController.
///
/// Responsibility:
///     The controller handles HTTP requests related to individual stories.
///     It acts as the entry point between the frontend and the backend logic.
///
/// What this controller does:
///     - Receive requests from the frontend
///     - Call backend services or repositories to retrieve story data
///     - Return results as JSON responses
///
/// Planned endpoints:
///     - GET /api/stories/{id}
///       Returns the full story when a user selects a story from search results.
///
/// This controller connects to:
///     - IStoryRepository (to retrieve story data from the database)
///     - DTO models (to control what data is returned to the frontend)
/// </summary>

[ApiController] // Tells ASP.NET that this class is an API controller that handles HTTP requests and returns JSON responses.
[Route("api/[controller]")] // Sets the base route for all endpoints in this controller, for example: /api/stories
public class StoriesController : ControllerBase
{
    // This field holds the repository instance used to retrieve story data from the database.
    private readonly IStoryRepository _storyRepository;

    // Constructor: ASP.NET injects the repository here using dependency injection.
    // This allows the controller to use the repository without creating it manually.
    public StoriesController(IStoryRepository storyRepository)
    {
        _storyRepository = storyRepository;
    }

    // GET /api/stories
    // Returns a list of all stories in the database.
    [HttpGet]
    public async Task<IActionResult> GetAllStories()
    {
        try
        {
            var stories = await _storyRepository.GetAllAsync();
            var storyDtos = stories.Select(story => new StoryDetailDTO
            {
                Id = story.Id,
                Title = story.Title,
                Author = story.Author,
                Year = story.Year,
                Genre = story.Genre,
                Content = story.Content
            });

            return Ok(storyDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "An internal server error occurred.");
        }
    }

    // GET /api/stories/{id}
    // This endpoint returns a single story by id.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetStoryById(int id)
    {
        try
        {
            // Ask the repository for the story with the given id.
            var story = await _storyRepository.GetByIdAsync(id);

            // If the story does not exist, return HTTP 404 Not Found.
            if (story == null)
            {
                return NotFound($"Story with id {id} was not found.");
            }

            // Map the Story entity to a DTO.
            var storyDto = new StoryDetailDTO
            {
                Id = story.Id,
                Title = story.Title,
                Author = story.Author,
                Year = story.Year,
                Genre = story.Genre,
                Content = story.Content
            };

            // If the story exists, return HTTP 200 OK with the story data.
            return Ok(storyDto);
        }
        catch (Exception ex)
        {
            // In a real application, you would log the exception here.
            // Then return a generic error message to the client.
            Console.WriteLine(ex);
            return StatusCode(500, "An internal server error occurred. - the developer");
        }
    }
}