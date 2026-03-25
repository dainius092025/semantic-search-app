
using Backend.Services.Interfaces;
using Backend.Services.DataLoading;
using Microsoft.Extensions.Logging;

namespace Backend.Services;


// This service orchestrates the entire ingestion process, from loading raw data to generating embeddings and summaries, and finally saving everything to the database. 
// It is designed to be efficient and scalable, leveraging asynchronous programming and parallel processing where appropriate.

public class IngestionService : IStoryIngestionService
{
    private readonly IStoryRepository _repository;
    private readonly IOllamaService _ollama;
    private readonly StoryDataLoader _dataLoader;
    private readonly ILogger<IngestionService> _logger;

    public IngestionService(IStoryRepository repository, IOllamaService ollama, ILogger<IngestionService> logger)
    {
        _repository = repository;
        _ollama = ollama;
        _dataLoader = new StoryDataLoader();
        _logger = logger;
    }

    public async Task RunFullIngestionAsync()
    {
        // 1. Load metadata and raw text from files
        var rawStories = await _dataLoader.LoadAllStoriesAsync();
        _logger.LogInformation("Loaded {Count} raw stories.", rawStories.Count);

        // 2. Process stories one by one
        foreach (var story in rawStories)
        {
            // Check if story is already fully ingested to avoid duplicates. We check for full ingestion (embedding + summary) not just existence, because a story could exist in the database but still be missing generated data if Ollama failed previously
            if (await _repository.IsFullyIngestedAsync(story.Id))
            {
                _logger.LogInformation("Skipping story {Id} - already fully ingested.", story.Id);
                continue;
            }

                // Generate Embedding
                var vector = await _ollama.GenerateEmbeddingAsync(story.Content);
                
                if (vector == null || vector.Length == 0)
                    {
                        _logger.LogError("Embedding generation failed for story {Id}. Skipping.", story.Id);
                        continue;
                    }

                story.Embedding = new Pgvector.Vector(vector);

                // Generate Summary only if content is not empty
                //This prevents sending empty text to the LLM, which can produce confusing responses.
                if (!string.IsNullOrWhiteSpace(story.Content))
                {
                    // Generate Summary
                    story.Summary = await _ollama.GenerateSummaryAsync(story.Content);
                    
                    if (string.IsNullOrWhiteSpace(story.Summary))
                    {
                        _logger.LogError("Summary generation failed for story {Id}. Skipping.", story.Id);
                        continue;
                    }
                    
                }
                else
                {
                    //just because it is nice to see in the console when something is wrong with the data, instead of just getting empty summaries that can be confusing when debugging
                    story.Summary = "No summary available.";
                    _logger.LogWarning("Story {Id} has empty content. Summary set to default.", story.Id);
                }

                // 3. Save to Database
                await _repository.AddAsync(story);
                //just because it is nice to see in the console when something is wrong with the data, instead of just getting empty summaries that can be confusing when debugging
                _logger.LogInformation("Ingested story {Id} successfully.", story.Id);
        }
    }

    // Checks whether all stories in the database are fully ingested. We load all stories and check each one against the repository. If any story is missing embedding or summary data, we consider ingestion incomplete and return false. Returns true only when every story is fully ingested.
    public async Task<bool> IsIngestionCompleteAsync()
    {
        // Get all stories from the database
        var allStories = await _repository.GetAllAsync();

        // Check each story for completeness
        foreach (var story in allStories)
        {
            // If any story is not fully ingested, return false immediately
            if (!await _repository.IsFullyIngestedAsync(story.Id))
            {
                return false;
            }
        }

        // All stories are fully ingested
        return true;
    }
}