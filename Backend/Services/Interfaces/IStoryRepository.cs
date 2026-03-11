/// <summary>
/// Defines the contract for all database operations related to stories.
/// Any class implementing this interface must provide methods to:
/// - save stories
/// - search stories using embedding vectors
/// - retrieve stories by ID
/// - check if a story already exists
/// 
/// This interface acts as the main gateway between the application logic
/// and the database layer for all story-related data.
/// </summary>


using Backend.Models;

namespace Backend.Services.Interfaces
{
    public interface IStoryRepository
    { 
        // Saves a new story to the database and is usedwhen seedingthe database with stories
        // Task         = asynchronous operation
        // AddAsync     = saves a new story to the database
        // Story story  = accepts a Story object to save
        Task AddAsync(Story story);

        // Searches the database for stories that match the meaning of a search query. Takes an embedding vector and returns a list of matching stories.
        // Used by the search endpoint.
        // IEnumerable<(Story Story, double Similarity)> = returns a list of tuples, each with a story and its similarity score.
        // SearchAsync             = searches the database
        // float[] embedding       = accepts a vector to compare against
       Task<IEnumerable<(Story Story, double Similarity)>> SearchAsync(float[] embedding, int limit);

        // Gets one story from the database by its ID. Returns null if the story does not exist.
        // Used by the story detail endpoint.
        // Task<Story>  = returns a single story
        // GetByIdAsync = finds a story by its ID
        // int id       = accepts the story ID
        Task<Story?> GetByIdAsync(int id);

        // Checks if a story already exists in the database. Returns true if it exists, false if it does not.
        // Used by the seeder to avoid saving duplicate stories.
        // Task<bool>   = returns true or false
        // ExistsAsync  = checks if story already exists
        // int id       = accepts the story ID to check
        Task<bool> ExistsAsync(int id);
    }
} 