using System;

namespace Backend.Models.DTOs;

// The incoming request from the React SearchBar
public class SearchRequestDTO
{
    public string Query { get; set; } = string.Empty; // The "User Intent"
    public int Limit { get; set; } = 5;               // Default matches to return
}
