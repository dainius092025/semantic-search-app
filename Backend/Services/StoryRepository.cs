using Backend.Models;
using Backend.Services.Interfaces;
using Backend.Services.DataLoading;
using Backend.Services.Search;

namespace Backend.Services

// Note: This is just an "Orchestrator" - the actual logic for loading and searching stories is delegated to StoryDataLoader and StorySearchService, respectively.

// Temporary in-memory repository - will be replaced with a proper database implementation.
// This class is responsible for managing story data, including loading from files and performing searches.
// It uses StoryDataLoader to load story metadata and content, and StorySearchService
// to perform semantic searches based on embeddings.
// The repository is designed to be thread-safe and efficient, loading all stories into memory on first access and caching them 
// for subsequent operations.
// Future improvements may include adding methods for updating and deleting stories, as well as implementing more advanced search capabilities
// Note: This implementation assumes that the story metadata and content are stored in a specific format in the Docs/Stories directory,
// and that the metadata.json file contains an array of story metadata objects.
// The StoryRepository class implements the IStoryRepository interface, providing methods for adding new stories, searching for stories 
// based on embeddings, and retrieving stories by ID.

{
    public class StoryRepository : IStoryRepository
    {
        private readonly StoryDataLoader _dataLoader;
        private readonly StorySearchService _searchService;
        private List<Story> _stories = new();
        private bool _isLoaded = false;

        public StoryRepository()
        {
            _dataLoader = new StoryDataLoader();
            _searchService = new StorySearchService();
        }

        private async Task LoadAllStoriesAsync()
        {
            if (_isLoaded) return;
            _stories = await _dataLoader.LoadAllStoriesAsync();
            _isLoaded = true;
        }

        public async Task AddAsync(Story story)
        {
            await LoadAllStoriesAsync();
            _stories.Add(story);
        }

        public async Task<IEnumerable<Story>> SearchAsync(float[] embedding)
        {
            await LoadAllStoriesAsync();
            return _searchService.SearchByEmbedding(_stories, embedding);
        }

        public async Task<Story?> GetByIdAsync(int id)
        {
            await LoadAllStoriesAsync();
            return _stories.FirstOrDefault(s => s.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            await LoadAllStoriesAsync();
            return _stories.Any(s => s.Id == id);
        }
    }
}