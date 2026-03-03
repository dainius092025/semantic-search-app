import { useState, useCallback } from "react";
import { searchStories, SearchMode } from "../api/stories";

export function useSearch() {
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [hasSearched, setHasSearched] = useState(false);
  const [lastQuery, setLastQuery] = useState("");
  const [mode, setMode] = useState(SearchMode.Semantic);

  const search = useCallback(async (query, searchMode = mode, limit = 20) => {
    if (!query.trim()) return;
    setLoading(true);
    setError(null);
    setLastQuery(query);

    try {
      const data = await searchStories({ query: query.trim(), mode: searchMode, limit });
      setResults(data);
    } catch (err) {
      setError(err.message || "Something went wrong.");
      setResults([]);
    } finally {
      setLoading(false);
      setHasSearched(true);
    }
  }, [mode]);

  const reset = useCallback(() => {
    setResults([]);
    setHasSearched(false);
    setError(null);
    setLastQuery("");
  }, []);

  return { results, loading, error, hasSearched, lastQuery, search, reset, mode, setMode };
}
