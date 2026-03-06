using System;

namespace Backend.Models.DTOs;

public class StoryIngestionDTO
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Genre { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty; // To link to the .txt file (not sure yet)
}
