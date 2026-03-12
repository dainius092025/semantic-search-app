
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
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Skipping story {story.Id} - already exists in database.");
                Console.ResetColor();
                continue;
            }

                // Generate Embedding
                var vector = await _ollama.GenerateEmbeddingAsync(story.Content);
                story.Embedding = new Pgvector.Vector(vector);

                // Generate Summary only if content is not empty
                //This prevents sending empty text to the LLM, which can produce confusing responses.
                if (!string.IsNullOrWhiteSpace(story.Content))
                {
                    // Generate Summary
                    story.Summary = await _ollama.GenerateSummaryAsync(story.Content);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    story.Summary = "No summary available.";
                    Console.ResetColor();
                }

                // 3. Save to Database
                await _repository.AddAsync(story);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Ingested story {story.Id} successfully.");
                Console.ResetColor();
        }
    }
}