/// <summary>
///This file defines the StoriesController.
///Responsibility:
///     The controller handles HTTP requests related to individual stories.
///     It acts as the entry point between the frontend and the backend logic.
///What this controller will do:
///     - Receive requests from the frontend
///     - Call backend services or repositories to retrieve story data
///     - Return results as JSON responses
/// Planned endpoints:
///     - GET /api/stories/{id}: 
///     Returns the full story when a user selects a story from search results.
/// In the future this controller will connect to:
///     - IStoryRepository (to retrieve story data from the database)
///     - DTO models (to control what data is returned to the frontend)
/// 
/// </summary>

using Microsoft.AspNetCore.Mvc;
using Backend.Services.Interfaces;
using Backend.Models.DTOs;

namespace Backend.Controllers;

[ApiController]  //this bit tells ASP.NET tha this calss is an API COntroller, which means it will handle HTTP requests and return JSON responses.

[Route("api/[controller]")]//this sets the base route for all endpoints in this controller, for example: /api/stories

public class StoriesController : ControllerBase
{
    //at the moment its holds only testing, later i will add endpoints like: GET /api/stories/{id}
/*
    DELETE BEFORE WORKING FURTHER ON THIS CONTROLLER, THIS IS JUST A TEST ENDPOINT TO MAKE SURE THE CONTROLLER IS SET UP CORRECTLY AND CAN RESPOND TO REQUESTS. IT RETURNS A SIMPLE JSON MESSAGE WHEN YOU HIT /api/stories/test
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { message = "stories controllers is working!" });
    }
*/

    private readonly IStoryRepository _storyRepository; //this variable will hold the repository instance that we will use to interact with the database and retrieve story data.

    //Constructor, Asp.net automatically injects the repository here using dependency injection.
    //this allows the controller to access the repository without ctreating it manually
    public StoriesController(IStoryRepository storyRepository)
    {   //this is calss field
                          //This is the constructor parameter. comes from ASP.net dependancy injection
        _storyRepository = storyRepository; //Take the repository object ASP.NET gave us and store it inside the controller field.
    }

    //MY ENDPOINT 
    // GET /api/stories/{id}, this endpoint returns a single story by id

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStoryById(int id)
    {
        try
        {
            // ask the repository for the story with the given id
            var story = await _storyRepository.GetByIdAsync(id);

            //if the story does not exist, return http 404 not found
            if (story == null)
            {
                return NotFound();
            }

            var storyDto = new StoryDetailDTO // map to DTO
            {
                Id = story.Id,
                Title = story.Title,
                Author = story.Author,
                Year = story.Year,
                Genre = story.Genre,
                Content = story.Content
            };
            //if the story exists, return http 200 with the story data
            return Ok(storyDto);
        }
        catch (Exception ex)
        {
            // In a real application, you would log the full exception here for debugging.
            // For example: _logger.LogError(ex, "Error retrieving story with id {StoryId}", id);
            // Then, return a generic error message to the client.
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}