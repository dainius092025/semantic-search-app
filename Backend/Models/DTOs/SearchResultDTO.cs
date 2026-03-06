using System;

namespace Backend.Models.DTOs;

public class SearchResultDTO
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty; // The 1-2 sentence AI summary
    public double Similarity { get; set; }              // The Cosine Similarity score
}
