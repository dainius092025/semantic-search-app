
using Backend.Models;

/// <summary>
/// Defines the contract for services responsible for ingestion stories into the semanitc search system. The ignestion service coordinates the pipeline that prepares stories for search, such as processing story data and passing it to other components (for example: embedding generation and database storage). This interface specifies what ingestion operations are available, while the actual logic is implemented in StoryIngestionService.
/// </summary>
namespace Backend.Services.Interfaces;

public interface IStoryIngestionService
{
    //runs asynchronously, basically program can start this work and continue doing other things while waitng for it to finish
        //IngestStory+Async(process a story for the search system + method is asynchronous and will return a Task 
                         //method expects a story object as input, which contains the data that needs to be ingested into the search system
    Task IngestStoryAsync(Story story);
}





