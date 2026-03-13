const BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5041";

// SearchMode enum values from your API.
export const SearchMode = {
  Semantic: 0,
};

async function fetchJson(url, options, errorPrefix) {
  // Shared fetch wrapper keeps status handling consistent across endpoints.
  const res = await fetch(url, options);

  if (!res.ok) {
    throw new Error(`${errorPrefix}: ${res.status} ${res.statusText}`);
  }

  return res.json();
}

/**
 * POST /api/stories/search
 * Body: { query: string, mode: SearchMode (int), limit: number }
 * Returns: Story[]
 *   { id, title, author, genre, publishedYear, summary, content }
 */
export async function searchStories({
  query,
  mode = SearchMode.Semantic,
  limit = 20,
}) {
  const data = await fetchJson(
    `${BASE_URL}/api/stories/search`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ query, mode, limit }),
    },
    "Search failed"
  );

  // Response shape: [{ story: { id, title, ... }, score: number }, ...]
  // Flatten to: [{ id, title, ..., score }, ...]
  return data.map(({ story, score }) => ({ ...story, score }));
}

/**
 * GET /api/stories
 * Returns: Story[]
 */
export async function getAllStories() {
  return fetchJson(
    `${BASE_URL}/api/stories`,
    { headers: { accept: "application/json" } },
    "Failed to load stories"
  );
}

/**
 * GET /api/stories/{id}
 * Returns: Story
 */
export async function getStoryById(id) {
  return fetchJson(
    `${BASE_URL}/api/stories/${id}`,
    { headers: { accept: "application/json" } },
    "Story not found"
  );
}
