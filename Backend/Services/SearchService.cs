using Backend.Models;
using Backend.Models.DTOs;
using Backend.Services.Interfaces;

namespace Backend.Services;

// Service responsible for handling all search-related logic. it combines semantic search (embedding-based) and metadata search (keyword-based) and applies weighted ranking to return the most relevant results.
public class SearchService : ISearchService
{
    private readonly IOllamaService _ollamaService;
    private readonly IStoryRepository _storyRepository;

    // Defines weighting for hybrid ranking.
    // Semantic relevance is prioritized, but keyword matches still influence results.
    private const double SemanticWeight = 0.7;
    private const double KeywordWeight = 0.3;

    // Dependencies are injected via constructor:
    // - IOllamaService: generates embeddings for semantic search
    // - IStoryRepository: provides access to story data and search queries
    public SearchService(IOllamaService ollamaService, IStoryRepository storyRepository)
    {
        _ollamaService = ollamaService;
        _storyRepository = storyRepository;
    }

    // Performs hybrid search by combining semantic similarity and metadata matching. Semantic search finds stories based on meaning using embeddings, while metadata search finds exact matches (title, author, genre, etc.).
    public async Task<List<SearchResultDTO>> HybridSearchAsync(SearchRequestDTO request)
    {
        // Normalizing user input to make search consistent
        var normalizedQuery = request.Query
            .Trim()   
            .ToLower();

        // improved query for semantic understanding (i read is it better for themes/feelings)
        var enrichedQuery = $"A short story about the theme, feeling, or situation of {normalizedQuery}";


         // Firstly we convert the user's query into an embedding vector using the Ollama service. This allows us to compare the meaning of the query with stored story embeddings.         
        var embedding = await _ollamaService.GenerateEmbeddingAsync(enrichedQuery, EmbeddingTask.Query);

        // Then we perform semantic search in the database and return stories with a similarity score based on how close they are to the query embedding.
        var semanticResults = await _storyRepository.SearchAsync(embedding, request.Limit);

        // Now we do metadata (keyword-based) search.This finds stories that match exact words or phrases in fields like title, author
        var metadataResults = await _storyRepository.SearchByMetadataAsync(normalizedQuery);

        // collecting all unique storu IDs from both semantic(comes from vector similarity) and metadata (comes from keyword matching) results. SO using UNION ensures that stories appearing in both results sets are not diplicates and found by only one method are still included.Thi creates a unified set of conditions for hybrid ranking.
        var allStoryIds = semanticResults
            .Select(result => result.Story.Id)
            .Union(metadataResults.Select(story => story.Id))
            .ToList();

        //Build one combined result list from all unique story IDs. thisa allows hybrid search include: stories found by both methods, semantic-only stories, metadata-only stories.
        var combinedResults = allStoryIds
            .Select(id =>
            {
                //From the list of semantic results, find the first item where the story’s ID matches the given ID. If none is found, return a default value.                                  
                                                                    //Take each result, and check if its story ID equals the ID we are looking for
                                                    //first item that matches the condition, or a default value if nothing matches.
                                    //From the list of semantic results
                var semanticMatch = semanticResults.FirstOrDefault(result => result.Story.Id == id);

                var metadataMatch = metadataResults.FirstOrDefault(story => story.Id == id);

                var hasSemantic = semanticResults.Any(result => result.Story.Id == id);
                //If we have a semantic match, use that story. Otherwise, use the metadata match.
                var story = hasSemantic ? semanticMatch.Story : metadataMatch!;

                return new SearchResultDTO
                {
                    Id = story.Id,
                    Title = story.Title,
                    Author = story.Author,
                    Year = story.Year,
                    Genre = story.Genre,
                    Summary = story.Summary,
                    Similarity = hasSemantic ? semanticMatch.Similarity : 0
                };
            })
            .ToList();


        // we need to combine semantic and keyword results using weighted ranking. Semantic score represents meaning similarity (0–1), keyword score is binary (1 or 0, exists or not). The final score is calculated as:
        // 70% semantic relevance + 30% keyword relevance.
        foreach (var dto in combinedResults)
        {   
            // Preserve the original semantic similarity score
            var semanticScore = dto.Similarity;

            // Check if the current story also appears in metadata search results. If yesthen strong keyword match (1), otherwise no match (0)
            var keywordScore = metadataResults.Any(story => story.Id == dto.Id)
                ? 1.0
                : 0.0;
                

            /* // Combine both scores into a final relevance score
            var finalScore = (semanticScore * SemanticWeight) + (keywordScore * KeywordWeight); */

            var finalScore = semanticScore;

            if (keywordScore > 0)
            {
                finalScore += 0.1; // small keyword bonus
            }

            finalScore = Math.Min(finalScore, 1.0);

            
            //trying to show stronger matches stand out more clearly, squaring the score keeps the ranking order, but spreads the values apart.

            // Converting to percentage (0–100) for better readability
            var percentage = finalScore * 100;
            percentage = Math.Min(percentage, 100);

            dto.Similarity = Math.Round(percentage, 2);
        }
        //finaly we organize results by final score (highest first) and limit the output
        return combinedResults
            .OrderByDescending(r => r.Similarity)
            .Take(request.Limit)
            .ToList();

    }


    // Performs metadata-based (keyword) search only.This search matches exact words or phrases in fields: title, author, genre, year, without using embeddings.
    public async Task<List<SearchResultDTO>> MetadataSearchAsync(SearchRequestDTO request)
    {   
        // Normalizing user input to make search consistent
        var normalizedQuery = request.Query
            .Trim()   
            .ToLower();

        // First we query the database for stories that match the keyword search
        var stories = await _storyRepository.SearchByMetadataAsync(normalizedQuery);

        // Thenwe map the results to DTOs and limit the number of returned items
        var results = stories
            .Take(request.Limit)
            .Select(story => new SearchResultDTO
            {
                Id = story.Id,
                Title = story.Title,
                Author = story.Author,
                Year = story.Year,
                Genre = story.Genre,
                Summary = story.Summary,

            // Metadata search does not calculate similarity, so we set it to 0 
                Similarity = 0
            })
            .ToList();
        return results;
    }
} 
