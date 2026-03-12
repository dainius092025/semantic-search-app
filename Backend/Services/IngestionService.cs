
using Backend.Services.Interfaces;
using Backend.Services.DataLoading;

namespace Backend.Services;


// This service orchestrates the entire ingestion process, from loading raw data to generating embeddings and summaries, and finally saving everything to the database. 
// It is designed to be efficient and scalable, leveraging asynchronous programming and parallel processing where appropriate.

public class IngestionService
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
        Console.WriteLine("Starting ingestion process...");
        // 1. Load metadata and raw text from files
        var rawStories = await _dataLoader.LoadAllStoriesAsync();

        // 2. Sequential Processing
        // We process stories one by one because the DbContext is not thread-safe.
        foreach (var story in rawStories)
        {
            try 
            {
                // Check if story already exists to avoid duplicates
                if (await _repository.ExistsAsync(story.Id))
                {
                    Console.WriteLine($"Story '{story.Title}' (ID: {story.Id}) already exists. Skipping.");
                    continue;
                }

                Console.WriteLine($"Processing story: {story.Title}...");

                // Generate Embedding
                var vector = await _ollama.GenerateEmbeddingAsync(story.Content);
                story.Embedding = new Pgvector.Vector(vector);

                // Generate Summary
                story.Summary = await _ollama.GenerateSummaryAsync(story.Content);

                // 3. Save to Database
                await _repository.AddAsync(story);
                Console.WriteLine($"Successfully ingested: {story.Title}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing story '{story.Title}': {ex.Message}");
            }
        }

        Console.WriteLine("Ingestion process completed.");
    }
}