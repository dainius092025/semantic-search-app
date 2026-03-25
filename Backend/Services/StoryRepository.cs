using Backend.Data;
using Backend.Models;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore; // Needed for .CosineDistance()

namespace Backend.Services;

public class StoryRepository : IStoryRepository
{
    private readonly AppDbContext _context;

    public StoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<(Story Story, double Similarity)>> SearchAsync(float[] embedding, int limit)
    {
        // Convert the float array from Ollama to a pgvector Vector
        var queryVector = new Pgvector.Vector(embedding);

        // This query runs on the database server.
        // 1. It calculates the "cosine distance" between the search query vector and each story's embedding.
        //    A smaller distance means a better match (0 = identical, >0 = different).
        // 2. It orders the results by this distance to get the best matches first.
        // 3. It takes the top 5 results.
        var results = await _context.Stories
            .Where(s => s.Embedding != null) // Ensure we only search stories that have an embedding
            .Select(s => new { Story = s, Distance = s.Embedding!.CosineDistance(queryVector) }) // The '!' is safer now
            .OrderBy(r => r.Distance)
            .Take(limit)
            .ToListAsync();

        // 4. It converts the list of results into the required tuple format (Story, Similarity).
        //    Similarity is calculated as 1 - distance.
        return results.Select(r => (r.Story, 1 - r.Distance));    
    }

    public async Task AddAsync(Story story)
    {
        _context.Stories.Add(story);
        await _context.SaveChangesAsync();
    }

    public async Task<Story?> GetByIdAsync(int id) => await _context.Stories.FindAsync(id);
    public async Task<bool> ExistsAsync(int id) => await _context.Stories.AnyAsync(s => s.Id == id);

    public async Task<Story?> GetRandomAsync()
    {
        // Generates a random sort order in the database using a GUID
        return await _context.Stories.OrderBy(s => Guid.NewGuid()).FirstOrDefaultAsync();
    }

    public async Task<bool> IsFullyIngestedAsync(int id)
    {
        // Checks if the story exists AND has both an embedding and a summary
        return await _context.Stories
            .AnyAsync(s => s.Id == id && s.Embedding != null && !string.IsNullOrWhiteSpace(s.Summary));
    }

    //searches only in metadata: title, author, year, genre,  using simple matching "contains"
    public async Task<List<Story>> SearchByMetadataAsync(string query)
    {
        var normalizedQuery = query.Trim().ToLower();
        
        // Try to parse the year, if it fails, yearQuery will be null
        int? yearQuery = int.TryParse(query, out int y) ? y : null;

        var results = await _context.Stories
            .Where (s => s.Title.ToLower().Contains(normalizedQuery) ||//i has to be case sensitive.
                    s. Author.ToLower().Contains(normalizedQuery) ||
                    (yearQuery.HasValue && s.Year == yearQuery.Value) ||
                    s.Genre.ToLower().Contains(normalizedQuery))
            .OrderByDescending(s => s.Title.ToLower().Contains(normalizedQuery))//title comes first
            .ThenByDescending(s => s.Author.ToLower().Contains(normalizedQuery))//then this
            .ThenByDescending(s => s.Genre.ToLower().Contains(normalizedQuery))//and then this
                    .ToListAsync();

        return results;
    }

    public async Task<IEnumerable<Story>> GetAllAsync()
    {
        return await _context.Stories.ToListAsync();
    }
}