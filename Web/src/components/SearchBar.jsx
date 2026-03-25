import { useState } from "react";
import { Link } from "react-router-dom";
import styles from "./SearchBar.module.css";

const SUGGESTIONS = [
  "a man facing the sea alone",
  "grief and memory",
  "community ritual gone wrong",
  "daydreaming and escape",
  "technology controlling people",
];

export default function SearchBar({ onSearch, loading }) {
  const [query, setQuery] = useState("");

  const handleSubmit = () => {
    if (query.trim() && !loading) {
      onSearch(query.trim());
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter") {
      handleSubmit();
    }
  };

  const handleSuggestion = (suggestion) => {
    setQuery(suggestion);
    onSearch(suggestion);
  };

  return (
    <div className={styles.wrapper}>
      <p className={styles.eyebrow}>
        <Link to="/">Story Discovery Engine</Link>
      </p>

      <h1 className={styles.headline}>
        Find stories by
        <br />
        <em>meaning or keywords</em>
      </h1>

      <p className={styles.sub}>
        Describe a theme, emotion, or situation - the engine finds the most
        relevant stories from the collection.
      </p>

      <div className={styles.inputRow}>
        <div className={styles.inputWrap}>
          <span className={styles.inputIcon}>
            <svg
              width="18"
              height="18"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="1.8"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <circle cx="11" cy="11" r="8" />
              <path d="m21 21-4.35-4.35" />
            </svg>
          </span>

          <input
            className={styles.input}
            type="text"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder="Type Here..."
            autoFocus
          />

          {query && (
            <button
              type="button"
              className={styles.clearBtn}
              onClick={() => setQuery("")}
              aria-label="Clear"
            >
              x
            </button>
          )}
        </div>

        <button
          type="button"
          className={styles.searchBtn}
          onClick={handleSubmit}
          disabled={!query.trim() || loading}
        >
          {loading ? <span className={styles.spinner} /> : "Discover"}
        </button>
      </div>

      <div className={styles.suggestions}>
        <span className={styles.suggestLabel}>TRENDING SEARCHES:</span>
        {SUGGESTIONS.map((suggestion) => (
          <button
            key={suggestion}
            type="button"
            className={styles.tag}
            onClick={() => handleSuggestion(suggestion)}
          >
            {suggestion}
          </button>
        ))}
      </div>
    </div>
  );
}

