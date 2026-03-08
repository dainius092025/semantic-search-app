
namespace Backend.Models.DTOs;

// This DTO represents the full details of a story.
// It is returned when the frontend requests one story by ID.
public class StoryDetailDTO
{
    public int Id { get; set; }                        // Unique story identifier

    public string Title { get; set; } = string.Empty;  // Story title

    public string Author { get; set; } = string.Empty; // Story author

    public int Year { get; set; }                      // Publication year

    public string Genre { get; set; } = string.Empty;  // Story genre

    public string Content { get; set; } = string.Empty; // Full story text

}