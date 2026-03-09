using Backend.Models;

namespace Backend.Services.Search;

    public class StorySearchService
    {
        public IEnumerable<Story> SearchByEmbedding(List<Story> stories, float[] embedding)
        {
            // TODO: Implement semantic search logic
            return stories;
        }
    }
