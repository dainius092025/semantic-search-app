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

    // Checks if a story is fully ingested by verifying that both the summary and embedding are present and valid. A story that exists but has an empty summary or empty embedding is not considered fully ingested.
    // Returns true if complete, false if incomplete or not found.
    public async Task<bool> IsFullyIngestedAsync(int id)
    {
        return await _context.Stories
            .AnyAsync(s => s.Id == id &&
                    s.Embedding != null &&
                    s.Summary != null &&
                    s.Summary != string.Empty);
    }

    //searches only in metadata: title, author, year, genre,  using simple matching "contains"
    public async Task<List<Story>> SearchByMetadataAsync(string query)
    {
        var normalizedQuery = query.Trim().ToLower();
        var results = await _context.Stories
            .Where (s => s.Title.ToLower().Contains(normalizedQuery) ||//i has to be case sensitive.
                    s. Author.ToLower().Contains(normalizedQuery) ||
                    s.Year.ToString() == query||
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

    // Retrieves one random story from the database. We use Guid.NewGuid() to assign a random value to each story,then order by that value and take the first result.Returns null if the database is empty.
    public async Task<Story?> GetRandomAsync()
    {
        return await _context.Stories
            .OrderBy(s => Guid.NewGuid())
            .FirstOrDefaultAsync();
    }   
}