

namespace Backend.Models.DTOs;

public class SearchResultDTO
{
    public int Id { get; set; } //front end needs the id to know which story to fetch when the user clicks on a search result
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Summary { get; set; } = string.Empty; // The 1-2 sentence AI summary
    public double Similarity { get; set; }              // The Cosine Similarity score
}
