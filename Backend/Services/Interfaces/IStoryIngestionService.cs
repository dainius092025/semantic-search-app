
using Backend.Models;

/// <summary>
/// Defines the contract for services responsible for ingestion stories into the semanitc search system. The ignestion service coordinates the pipeline that prepares stories for search, such as processing story data and passing it to other components (for example: embedding generation and database storage). This interface specifies what ingestion operations are available, while the actual logic is implemented in StoryIngestionService.
/// </summary>
namespace Backend.Services.Interfaces;

public interface IStoryIngestionService
{

    Task RunFullIngestionAsync(); // This method will run the entire ingestion process, from loading raw story data to generating embeddings and summaries, and finally saving everything to the database. It is designed to be called once to populate the system with stories, and can be scheduled to run periodically if needed.

    // Checks whether all stories in the database are fully ingested. Returns true if all stories have valid embeddings and summaries.Returns false if any stories are missing generated data.
    Task<bool> IsIngestionCompleteAsync();

}






