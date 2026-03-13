import { useCallback, useState } from "react";
import { searchStories, SearchMode } from "../api/stories";

export function useSearch() {
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [hasSearched, setHasSearched] = useState(false);
  const [lastQuery, setLastQuery] = useState("");
  const [mode, setMode] = useState(SearchMode.Semantic);

  // Centralized async search handler shared by page-level components.
  const search = useCallback(
    async (query, searchMode = mode, limit = 20) => {
      const trimmedQuery = query.trim();
      if (!trimmedQuery) return;

      setLoading(true);
      setError(null);
      setLastQuery(query);

      try {
        const data = await searchStories({
          query: trimmedQuery,
          mode: searchMode,
          limit,
        });
        // Keep API order intact so "default" sorting reflects backend relevance.
        setResults(data);
      } catch (err) {
        setError(err.message || "Something went wrong.");
        setResults([]);
      } finally {
        setLoading(false);
        setHasSearched(true);
      }
    },
    [mode]
  );

  const reset = useCallback(() => {
    // Return UI state to its initial "no search yet" shape.
    setResults([]);
    setHasSearched(false);
    setError(null);
    setLastQuery("");
  }, []);

  return {
    results,
    loading,
    error,
    hasSearched,
    lastQuery,
    search,
    reset,
    mode,
    setMode,
  };
}
