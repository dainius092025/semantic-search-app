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
}