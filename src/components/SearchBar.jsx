import { useState } from "react";
import { SearchMode } from "../api/stories";
import styles from "./SearchBar.module.css";

const SUGGESTIONS = [
  "a man facing the sea alone",
  "grief and memory",
  "community ritual gone wrong",
  "daydreaming and escape",
  "technology controlling people",
  "hope after devastation",
];

const MODE_OPTIONS = [
  { value: SearchMode.Semantic, label: "Semantic", desc: "Meaning-based" },
 
];

export default function SearchBar({ onSearch, loading, mode }) {
  const [query, setQuery] = useState("");

  const handleSubmit = () => {
    if (query.trim() && !loading) onSearch(query.trim(), mode);
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter") handleSubmit();
  };

  const handleSuggestion = (s) => {
    setQuery(s);
    onSearch(s, mode);
  };

  return (
    <div className={styles.wrapper}>
      <p className={styles.eyebrow}>Story Discovery Engine</p>
      <h1 className={styles.headline}>
        Find stories by<br />
        <em>meaning, not keywords</em>
      </h1>
      <p className={styles.sub}>
        Describe a theme, emotion, or situation — the engine finds the most relevant stories from the collection.
      </p>

    
        
      

      <div className={styles.inputRow}>
        <div className={styles.inputWrap}>
          <span className={styles.inputIcon}>
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round">
              <circle cx="11" cy="11" r="8"/><path d="m21 21-4.35-4.35"/>
            </svg>
          </span>
          <input
            className={styles.input}
            type="text"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder="Type Here…"
            autoFocus
          />
          {query && (
            <button className={styles.clearBtn} onClick={() => setQuery("")} aria-label="Clear">✕</button>
          )}
        </div>
        <button
          className={styles.searchBtn}
          onClick={handleSubmit}
          disabled={!query.trim() || loading}
        >
          {loading ? <span className={styles.spinner} /> : "Discover"}
        </button>
      </div>

      <div className={styles.suggestions}>
        <span className={styles.suggestLabel}>TRENDING SEARCHES:</span>
        {SUGGESTIONS.map((s) => (
          <button key={s} className={styles.tag} onClick={() => handleSuggestion(s)}>
            {s}
          </button>
        ))}
      </div>
    </div>
  );
}
