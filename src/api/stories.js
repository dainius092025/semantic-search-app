const BASE_URL = "http://localhost:5041";

// SearchMode enum values from your API
export const SearchMode = {
  Semantic: 0,
  
};

/**
 * POST /api/stories/search
 * Body: { query: string, mode: SearchMode (int), limit: number }
 * Returns: Story[]
 *   { id, title, author, genre, publishedYear, summary, content }
 */
export async function searchStories({ query, mode = SearchMode.Semantic, limit = 20 }) {
  const res = await fetch(`${BASE_URL}/api/stories/search`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ query, mode, limit }),
  });

  if (!res.ok) {
    throw new Error(`Search failed: ${res.status} ${res.statusText}`);
  }

  // Response shape: [{ story: { id, title, ... }, score: number }, ...]
  // Flatten to: [{ id, title, ..., score }, ...]
  const data = await res.json();
  return data.map(({ story, score }) => ({ ...story, score }));
}

/**
 * GET /api/stories
 * Returns: Story[]
 */
export async function getAllStories() {
  const res = await fetch(`${BASE_URL}/api/stories`, {
    headers: { accept: "application/json" },
  });

  if (!res.ok) {
    throw new Error(`Failed to load stories: ${res.status} ${res.statusText}`);
  }

  return res.json();
}

/**
 * GET /api/stories/{id}
 * Returns: Story
 */
export async function getStoryById(id) {
  const res = await fetch(`${BASE_URL}/api/stories/${id}`, {
    headers: { accept: "application/json" },
  });

  if (!res.ok) {
    throw new Error(`Story not found: ${res.status} ${res.statusText}`);
  }

  return res.json();
}
