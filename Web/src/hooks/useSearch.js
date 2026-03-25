import { useCallback, useState } from "react";
import { searchStories } from "../api/stories";

export function useSearch() {
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [hasSearched, setHasSearched] = useState(false);
  const [lastQuery, setLastQuery] = useState("");

  const search = useCallback(
    async (query, limit = 20) => {
      const trimmedQuery = query.trim();
      if (!trimmedQuery) return;

      setLoading(true);
      setError(null);
      setLastQuery(query);

      try {
        const data = await searchStories({
          query: trimmedQuery,
          limit,
        });
        setResults(data);
      } catch (err) {
        setError(err.message || "Something went wrong.");
        setResults([]);
      } finally {
        setLoading(false);
        setHasSearched(true);
      }
    },
    []
  );

  const reset = useCallback(() => {
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
  };
}

