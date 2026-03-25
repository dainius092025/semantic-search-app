import { useMemo, useState } from "react";
import StoryCard from "./StoryCard";
import styles from "./ResultsGrid.module.css";

export default function ResultsGrid({
  results,
  query,
  onStoryClick,
  genreFilter: genreFilterProp,
  onGenreChange,
  darkStoryTheme,
}) {
  const [genreFilterState, setGenreFilterState] = useState("all");
  const genreFilter = genreFilterProp ?? genreFilterState;
  const setGenreFilter = onGenreChange ?? setGenreFilterState;
  const [sortBy, setSortBy] = useState("default");

  const genres = useMemo(() => {
    const set = new Set(results.map((s) => s.genre).filter(Boolean));
    return ["all", ...Array.from(set).sort()];
  }, [results]);

  const filtered = useMemo(() => {
    let out = [...results];
    if (genreFilter !== "all") {
      out = out.filter((s) => s.genre === genreFilter);
    }

    switch (sortBy) {
      case "year_desc":
        out.sort((a, b) => b.publishedYear - a.publishedYear);
        break;
      case "year_asc":
        out.sort((a, b) => a.publishedYear - b.publishedYear);
        break;
      case "title":
        out.sort((a, b) => (a.title || "").localeCompare(b.title || ""));
        break;
      case "author":
        out.sort((a, b) => (a.author || "").localeCompare(b.author || ""));
        break;
      case "score":
        out.sort((a, b) => (b.score ?? 0) - (a.score ?? 0));
        break;
      default:
        break;
    }

    return out;
  }, [results, genreFilter, sortBy]);

  if (results.length === 0) {
    return (
      <div className={styles.empty}>
        <span className={styles.emptyIcon}>*</span>
        <h3>No stories found</h3>
        <p>Try rephrasing your search with different themes or emotions.</p>
      </div>
    );
  }

  return (
    <div className={styles.wrapper}>
      <div className={styles.toolbar}>
        <p className={styles.count}>
          <strong>{filtered.length}</strong>{" "}
          {filtered.length === 1 ? "result" : "results"} for <em>"{query}"</em>
        </p>
        <div className={styles.controls}>
          <select
            className={styles.select}
            value={genreFilter}
            onChange={(e) => setGenreFilter(e.target.value)}
          >
            {genres.map((g) => (
              <option key={g} value={g}>
                {g === "all" ? "All genres" : g}
              </option>
            ))}
          </select>
          <select
            className={styles.select}
            value={sortBy}
            onChange={(e) => setSortBy(e.target.value)}
          >
            <option value="default">Sort: Relevance</option>
            <option value="score">Sort: Score down</option>
            <option value="year_desc">Sort: Newest first</option>
            <option value="year_asc">Sort: Oldest first</option>
            <option value="title">Sort: Title A-Z</option>
            <option value="author">Sort: Author A-Z</option>
          </select>
        </div>
      </div>

      {filtered.length === 0 ? (
        <div className={styles.empty}>
          <p>No results match the selected genre.</p>
        </div>
      ) : (
        <div className={styles.grid}>
          {filtered.map((story, i) => (
            <StoryCard
              key={story.id}
              story={story}
              index={i}
              onClick={() => onStoryClick(story)}
              dark={darkStoryTheme}
            />
          ))}
        </div>
      )}
    </div>
  );
}

