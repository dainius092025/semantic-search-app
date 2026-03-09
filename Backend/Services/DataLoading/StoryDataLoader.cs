using Backend.Models;
using System.Text.Json;


namespace Backend.Services.DataLoading
{
    public class StoryDataLoader
    {
        private readonly string _metadataPath;
        private readonly string _storiesPath;

        public StoryDataLoader()
        {/*
        it was looking for the metadata.json file in the Backend folder, 
        but since the Docs folder is outside of Backend, it could not find it. 
        I adjusted the file path logic to go up one level from the Backend folder 
        to the project root, and then down into Docs/Stories to find the metadata.json 
        file. This way, it can correctly locate the metadata and story files regardless 
        of where the application is run from.
            _metadataPath = Path.Combine(Directory.GetCurrentDirectory(), "Docs", "Stories", "metadata.json");
            _storiesPath = Path.Combine(Directory.GetCurrentDirectory(), "Docs", "Stories");
        */
        //Adjusted file path logic so the application correctly locates 
        // Docs/Stories/metadata.json from the project root instead of the Backend folder
            var projectRoot = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName;

            _metadataPath = Path.Combine(projectRoot, "Docs", "Stories", "metadata.json");
            _storiesPath = Path.Combine(projectRoot, "Docs", "Stories");
        }

        public async Task<List<Story>> LoadAllStoriesAsync()
        {
            var stories = new List<Story>();
            var json = await File.ReadAllTextAsync(_metadataPath);
            // Configure JSON deserialization so property names are matched without 
            // case sensitivity. Our metadata.json uses lowercase names 
            // (id, title, author, etc.), while the C# Story class uses PascalCase 
            // (Id, Title, Author).This option allows the JSON fields to map correctly 
            // to the Story properties.
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // This allows the deserializer to match JSON properties to C# properties regardless of case
            };
            var metadata = JsonSerializer.Deserialize<List<Story>>(json, options);

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