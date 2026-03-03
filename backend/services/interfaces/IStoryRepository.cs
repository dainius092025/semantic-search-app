/* This file defines the "contract" for all database operations related to stories any class that implemets this interface  MUST have these methods. This is the most important interface in the project - everypart of the BE that needs data goes through here. */

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
        // IEnumerable<Story>      = returns a list of stories
        // SearchAsync             = searches the database
        // float[] embedding       = accepts a vector to compare against
       Task<IEnumerable<Story>> SearchAsync(float[] embedding);

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