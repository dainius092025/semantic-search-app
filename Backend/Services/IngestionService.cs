
using Backend.Services.Interfaces;
using Backend.Services.DataLoading;

namespace Backend.Services;


// This service orchestrates the entire ingestion process, from loading raw data to generating embeddings and summaries, and finally saving everything to the database. 
// It is designed to be efficient and scalable, leveraging asynchronous programming and parallel processing where appropriate.

public class IngestionService : IStoryIngestionService
{
    private readonly IStoryRepository _repository;
    private readonly IOllamaService _ollama;
    private readonly StoryDataLoader _dataLoader;

    public IngestionService(IStoryRepository repository, IOllamaService ollama)
    {
        _repository = repository;
        _ollama = ollama;
        _dataLoader = new StoryDataLoader();
    }

    public async Task RunFullIngestionAsync()
    {
        // 1. Load metadata and raw text from files
        var rawStories = await _dataLoader.LoadAllStoriesAsync();

        // 2. Process stories one by one
        foreach (var story in rawStories)
        {
            // Check if story already exists to avoid duplicates
            if (await _repository.ExistsAsync(story.Id))
                continue;

            // Generate Embedding
            var vector = await _ollama.GenerateEmbeddingAsync(story.Content);
            story.Embedding = new Pgvector.Vector(vector);

            // Generate Summary
            story.Summary = await _ollama.GenerateSummaryAsync(story.Content);

            // 3. Save to Database
            await _repository.AddAsync(story);
        }
    }
}