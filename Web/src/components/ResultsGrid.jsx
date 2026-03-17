import { useMemo, useState } from "react";
import StoryCard from "./StoryCard";
import styles from "./ResultsGrid.module.css";

function sortStories(stories, sortBy) {
  // Sort in-place on a cloned array supplied by caller.
  switch (sortBy) {
    case "year_desc":
      stories.sort((a, b) => b.publishedYear - a.publishedYear);
      break;
    case "year_asc":
      stories.sort((a, b) => a.publishedYear - b.publishedYear);
      break;
    case "title":
      stories.sort((a, b) => (a.title || "").localeCompare(b.title || ""));
      break;
    case "author":
      stories.sort((a, b) => (a.author || "").localeCompare(b.author || ""));
      break;
    case "score":
      stories.sort((a, b) => (b.score ?? 0) - (a.score ?? 0));
      break;
    default:
      // Keep API order (relevance).
      break;
  }

  return stories;
}

export default function ResultsGrid({
  results,
  query,
  onStoryClick,
  genreFilter: genreFilterProp,
  onGenreChange,
}) {
  const [genreFilterInternal, setGenreFilterInternal] = useState("all");
  const [sortBy, setSortBy] = useState("default");
  const genreFilter = genreFilterProp ?? genreFilterInternal;
  const setGenreFilter = onGenreChange ?? setGenreFilterInternal;

  // Build a stable genre filter list from the current result set.
  const genres = useMemo(() => {
    const set = new Set(results.map((story) => story.genre).filter(Boolean));
    return ["all", ...Array.from(set).sort()];
  }, [results]);

  const filtered = useMemo(() => {
    // Always clone before sorting to keep props immutable.
    let output = [...results];

    if (genreFilter !== "all") {
      output = output.filter((story) => story.genre === genreFilter);
    }

    return sortStories(output, sortBy);
  }, [results, genreFilter, sortBy]);

  if (results.length === 0) {
    return (
      <div className={styles.empty}>
        <span className={styles.emptyIcon}>â§</span>
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
            {genres.map((genre) => (
              <option key={genre} value={genre}>
                {genre === "all" ? "All genres" : genre}
              </option>
            ))}
          </select>

          <select
            className={styles.select}
            value={sortBy}
            onChange={(e) => setSortBy(e.target.value)}
          >
            <option value="default">Sort: Relevance </option>
            <option value="score">Sort: Score “</option>
            <option value="year_desc">Sort: Newest first</option>
            <option value="year_asc">Sort: Oldest first</option>
            <option value="title">Sort: Title </option>
            <option value="author">Sort: Author</option>
          </select>
        </div>
      </div>

      {filtered.length === 0 ? (
        <div className={styles.empty}>
          <p>No results match the selected genre.</p>
        </div>
      ) : (
        <div className={styles.grid}>
          {filtered.map((story, index) => (
            <StoryCard
              key={story.id}
              story={story}
              index={index}
              onClick={() => onStoryClick(story)}
            />
          ))}
        </div>
      )}
    </div>
  );
}
