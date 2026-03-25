const BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5162";

async function fetchJson(url, options, errorPrefix) {
  const res = await fetch(url, options);

  if (!res.ok) {
    let errorMsg = `${errorPrefix}: ${res.status} ${res.statusText}`;
    try {
      const errorData = await res.text();
      if (errorData) errorMsg += ` - ${errorData}`;
    } catch (e) {}
    throw new Error(errorMsg);
  }

  return res.json();
}

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

  return data.map((item) => ({
    ...item,
    score: item.similarity,
    publishedYear: item.year,
  }));
}

export async function getAllStories() {
  const data = await fetchJson(
    `${BASE_URL}/api/Stories`,
    { headers: { accept: "application/json" } },
    "Failed to load stories"
  );

  return data.map((item) => ({
    ...item,
    score: item.similarity,
    publishedYear: item.year,
  }));
}

export async function getStoryById(id) {
  const data = await fetchJson(
    `${BASE_URL}/api/Stories/${id}`,
    { headers: { accept: "application/json" } },
    "Story not found"
  );

  return {
    ...data,
    score: data.similarity,
    publishedYear: data.year,
  };
}

