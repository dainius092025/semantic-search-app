// Temporary placeholder - other developers will complete this

using System.Text.Json.Serialization;
namespace Backend.Models;

public class Story
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;

    [JsonPropertyName("published_year")]
    public int Year { get; set; }
    public string Genre { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public float[] Embedding { get; set; } = Array.Empty<float>();
}