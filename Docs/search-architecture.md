# Search Architecture

## What This Document Covers
This document explains how search works in this application.
The system supports two types of search:
- **Semantic search** — finds stories by meaning
- **Keyword search** — finds stories by exact words
- **Hybrid search** — combines both for better results

## Semantic Search

Semantic search finds stories based on meaning, not exact words.
For example, searching "lonely person" could return a story about 
isolation even if those exact words are never used.

### How it works
1. User enters a search query
2. The query is sent to Ollama (a local AI service)
3. Ollama converts the query into a list of numbers called an **embedding vector**
4. The database compares this vector against all stored story vectors
5. Stories with the closest meaning are returned first

### Where it lives in the code
- Embedding generation: `OllamaService.GenerateEmbeddingAsync()`
- Database search: `StoryRepository.SearchAsync()`
- Uses **cosine similarity** to measure how close two vectors are in meaning
- The semantic similarity score ranges from 0.0 to 1.0
- A higher score means the story meaning is closer to the search query
- This raw score is later combined with the keyword score 
  in hybrid search to produce the final result score
- This score has nothing to do with matching exact words

## Keyword Search

Keyword search finds stories by matching exact words or phrases
against story metadata. It is fast and predictable — if the word
exists in the data, it will be found.

### What it searches
- Title
- Author
- Genre
- Year

### How it works
1. User enters a search query
2. The query is compared directly against story metadata fields
3. Stories where any field contains the search word are returned
4. Results are ordered by relevance — title matches come first,
   then author, then genre

### Score
Keyword search uses a binary score:
- **1** = story matched the keyword query
- **0** = story did not match the keyword query

This score is not shown directly to the user.
It is used internally as part of the hybrid search 
ranking calculation.

### Where it lives in the code
- Database search: `StoryRepository.SearchByMetadataAsync()`
- Uses SQL LIKE matching via Entity Framework `.Contains()`
- Search is case insensitive
- Year requires an exact match (e.g. "1984")
- Does not use AI or embeddings — purely text matching

## Hybrid Search

Hybrid search combines semantic search and keyword search
to produce better results than either method alone.
It is the default search method used when a user searches
from the main search bar.

### How it works
1. User enters a search query
2. Both semantic search and keyword search run at the same time
3. Results from both searches are combined into one list
4. Each story receives a final score based on both results
5. Stories are ranked by final score, highest first
6. Top results are returned to the frontend

### Scoring
The final score is calculated using weighted ranking:
- **70%** semantic similarity score (meaning match)
- **30%** keyword score (exact word match)

For example:
- A story with semantic score 0.80 and a keyword match:
  (0.80 × 0.70) + (1 × 0.30) = **0.86 final score**
- A story with semantic score 0.80 and no keyword match:
  (0.80 × 0.70) + (0 × 0.30) = **0.56 final score**

### Where it lives in the code
- Main logic: `SearchService.HybridSearchAsync()`
- Semantic weight: 0.7 (defined as a constant)
- Keyword weight: 0.3 (defined as a constant)
- Final score is returned to the frontend as `Similarity`

## Example Search Flow

Here is a complete example of what happens when a user 
searches for "lonely person at sea":

### Step 1 — Frontend
- User types "lonely person at sea" in the search bar
- Frontend sends a POST request to `/api/search`
- Request body: `{ "query": "lonely person at sea", "limit": 5 }`

### Step 2 — Controller
- `SearchController` receives the request
- Validates that the query is not empty and at least 3 characters
- Passes the request to `SearchService`

### Step 3 — Semantic Search
- `SearchService` sends the query to `OllamaService`
- Ollama converts "lonely person at sea" into an embedding vector
- `StoryRepository` compares this vector against all stored stories
- Returns stories ranked by cosine similarity score

### Step 4 — Keyword Search
- `StoryRepository` searches title, author, genre and year
- Checks if any field contains "lonely person at sea"
- Returns any exact matches found

### Step 5 — Hybrid Ranking
- Results from both searches are combined
- Each story receives a final score:
  - 70% from semantic similarity
  - 30% from keyword match
- Stories are sorted by final score, highest first

### Step 6 — Response
- Top 5 results are returned to the frontend as JSON
- Each result includes title, author, year, genre, 
  summary and final similarity score
- Frontend displays results as story cards

## Architecture Summary

### Component Overview

| Component | Location | Responsibility |
|---|---|---|
| `SearchController` | Controllers/SearchController.cs | Receives HTTP requests, validates input |
| `SearchService` | Services/SearchService.cs | Orchestrates hybrid search and ranking |
| `StoryRepository` | Services/StoryRepository.cs | Runs database queries |
| `OllamaService` | Services/OllamaService.cs | Generates embedding vectors |

### Search Flow Diagram

Frontend
   ↓
SearchController (validates request)
   ↓
SearchService (orchestrates search)
   ↓                    ↓
OllamaService      StoryRepository
(embedding)        (semantic + keyword queries)
   ↓                    ↓
SearchService (combines + ranks results)
   ↓
Frontend (receives ranked results)

### Design Decisions

| Decision | Reason |
|---|---|
| 70% semantic, 30% keyword weighting | Meaning match is more important than exact words |
| Keyword search on metadata only | Full text keyword search would be too broad |
| Hybrid as default search | Gives best results for most user queries |
| Separate metadata endpoint | Allows frontend to use keyword search independently |