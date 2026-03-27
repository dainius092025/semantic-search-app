using Backend.Models.DTOs;

namespace Backend.Services.Interfaces;

// Defines the contract for search functionality in the application. any class implementing this interface must provide methods for hybrid (semantic + keyword) search and metadata-only search.
public interface ISearchService
{
    // Performs hybrid search by combining semantic similarity (embeddings)and metadata-based keyword matching, returning ranked results
    Task<List<SearchResultDTO>> SemanticSearchWithKeywordBoostAsync(SearchRequestDTO request);

    // Performs metadata-only search using direct keyword matching, e.g., title, author, genre, year without semantic embeddings.
        Task<List<SearchResultDTO>> MetadataSearchAsync(SearchRequestDTO request);
}