const BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5162";

/**
 * Shared fetch wrapper to keep status handling consistent across endpoints.
 */
async function fetchJson(url, options, errorPrefix) {
  const res = await fetch(url, options);

  if (!res.ok) {
    let errorMsg = `${errorPrefix}: ${res.status} ${res.statusText}`;
    try {
      const errorData = await res.text();
      if (errorData) errorMsg += ` - ${errorData}`;
    } catch (e) {
      // ignore
    }
    throw new Error(errorMsg);
  }

  return res.json();
}

/**
 * POST /api/Search
 * Body: { query: string, limit: number }
 * Returns: SearchResultDTO[]
 *   { id, title, author, year, summary, similarity }
 */
export async function searchStories({
  query,
  limit = 20,
}) {
  const data = await fetchJson(
    `${BASE_URL}/api/Search`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ query, limit }),
    },
    "Search failed"
  );

  // Response shape: [{ id, title, author, year, summary, similarity }, ...]
  // Normalize fields to match what components expect (e.g., similarity -> score)
  return data.map((item) => ({
    ...item,
    score: item.similarity,
    publishedYear: item.year // components might expect publishedYear
  }));
}

/**
 * GET /api/Stories
 * Returns: StoryDetailDTO[]
 */
export async function getAllStories() {
  const data = await fetchJson(
    `${BASE_URL}/api/Stories`,
    { headers: { accept: "application/json" } },
    "Failed to load stories"
  );

  return data.map(item => ({
    ...item,
    publishedYear: item.year
  }));
}

/**
 * GET /api/Stories/{id}
 * Returns: StoryDetailDTO
 */
export async function getStoryById(id) {
  const data = await fetchJson(
    `${BASE_URL}/api/Stories/${id}`,
    { headers: { accept: "application/json" } },
    "Story not found"
  );

  return {
    ...data,
    publishedYear: data.year
  };
}
