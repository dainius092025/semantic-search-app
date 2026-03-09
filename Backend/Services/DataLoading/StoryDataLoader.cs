using Backend.Models;
using System.Text.Json;


namespace Backend.Services.DataLoading
{
    public class StoryDataLoader
    {
        private readonly string _metadataPath;
        private readonly string _storiesPath;

        public StoryDataLoader()
        {
            _metadataPath = Path.Combine(Directory.GetCurrentDirectory(), "Docs", "Stories", "metadata.json");
            _storiesPath = Path.Combine(Directory.GetCurrentDirectory(), "Docs", "Stories");
        }

        public async Task<List<Story>> LoadAllStoriesAsync()
        {
            var stories = new List<Story>();
            var json = await File.ReadAllTextAsync(_metadataPath);
            var metadata = JsonSerializer.Deserialize<List<Story>>(json);

            if (metadata != null)
            {
                foreach (var meta in metadata)
                {
                    stories.Add(new Story
                    {
                        Id = meta.Id,
                        Title = meta.Title,
                        Author = meta.Author,
                        Year = meta.Year,
                        Genre = meta.Genre,
                        Content = LoadStoryContent(meta.Title),
                        Summary = string.Empty
                    });
                }
            }
            return stories;
        }

        public string LoadStoryContent(string title)
        {
            var filename = title.Replace(" ", "_") + ".txt";
            var path = Path.Combine(_storiesPath, filename);
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }
    }
}