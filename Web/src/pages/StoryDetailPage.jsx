// React hooks for state and lifecycle.
import { useEffect, useState } from "react";
// Router hooks for navigation and route params.
import { useLocation, useNavigate, useParams } from "react-router-dom";
// API helper to fetch a story by its id.
import { getStoryById } from "../api/stories";
// CSS module for the detail page.
import styles from "./StoryDetailPage.module.css";

// Map genre names to badge background/text colors.
const GENRE_COLORS = {
  "Literary Fiction": { bg: "#f0f4ff", color: "#3730a3" },
  "Science Fiction": { bg: "#ecfdf5", color: "#065f46" },
  Mystery: { bg: "#fdf4ff", color: "#7e22ce" },
  Fantasy: { bg: "#fff7ed", color: "#c2410c" },
  "Contemporary Drama": { bg: "#fef9c3", color: "#854d0e" },
  "Tech Thriller": { bg: "#f0fdf4", color: "#166534" },
  "Magical Realism": { bg: "#fdf2f8", color: "#9d174d" },
  "Historical Fiction": { bg: "#fef3c7", color: "#92400e" },
  "Post-Apocalyptic": { bg: "#f1f5f9", color: "#334155" },
  Humor: { bg: "#fef9c3", color: "#713f12" },
};

// Resolve a genre badge style, falling back to neutral colors.
function getGenreStyle(genre) {
  return GENRE_COLORS[genre] || {
    bg: "var(--parchment-dark)",
    color: "var(--text-muted)",
  };
}

// Convert story content into paragraphs and line breaks.
function renderContent(content) {
  // If no content is available, show a placeholder.
  if (!content) {
    return <p className={styles.noContent}>Full content not available.</p>;
  }

  // Keep backend line breaks as visual paragraph spacing in the reader view.
  return content.split("\n").map((para, i) =>
    para.trim() ? <p key={i}>{para}</p> : <br key={i} />
  );
}

// Story detail page fetches and displays one story.
export default function StoryDetailPage() {
  // Imperative navigation helper.
  const navigate = useNavigate();
  // Location may include preloaded story state.
  const { state } = useLocation();
  // URL param with the story id.
  const { id } = useParams();

  // Use story from router state if available, otherwise fetch from API.
  // Story data is initialized from router state when present.
  const [story, setStory] = useState(state?.story || null);
  // Loading state starts true if we need to fetch data.
  const [loading, setLoading] = useState(!state?.story);
  // Error message for failed fetches.
  const [error, setError] = useState(null);

  // Fetch story data when arriving without router state.
  useEffect(() => {
    // Direct visits to /story/:id do not carry router state, so fetch by id.
    if (!state?.story) {
      getStoryById(id)
        .then(setStory)
        .catch((err) => setError(err.message))
        .finally(() => setLoading(false));
    }
    // Re-run if id or presence of router state changes.
  }, [id, state?.story]);

  // Loading view while data is being fetched.
  if (loading) {
    return (
      <div className={styles.center}>
        <div className={styles.spinner} />
        <p>Loading storyâ€¦</p>
      </div>
    );
  }

  // Error view when the story is missing or fetch failed.
  if (error || !story) {
    return (
      <div className={styles.notFound}>
        <span className={styles.notFoundIcon}>â§</span>
        <h2>Story not found</h2>
        <p>{error || "This story isn't available."}</p>
        <button className={styles.backBtn} onClick={() => navigate("/")}>
          â† Back to Search
        </button>
      </div>
    );
  }

  // Compute styling for the genre badge.
  const genreStyle = getGenreStyle(story.genre);

  // Main detail page layout.
  return (
    <div className={styles.page}>
      <div className={styles.container}>
        {/* Back button to previous page in history. */}
        <button className={styles.back} onClick={() => navigate(-1)}>
          <svg
            width="16"
            height="16"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
          >
            <path d="M19 12H5M12 5l-7 7 7 7" />
          </svg>
          Back to results
        </button>

        <header className={styles.header}>
          <div className={styles.badges}>
            {/* Genre badge uses dynamic colors. */}
            <span
              className={styles.genreBadge}
              style={{ background: genreStyle.bg, color: genreStyle.color }}
            >
              {story.genre}
            </span>
            {/* Published year badge. */}
            <span className={styles.yearBadge}>{story.publishedYear}</span>
            {/* Story id badge for reference. */}
            <span className={styles.idBadge}>Story #{id}</span>
          </div>

          {/* Story title and author. */}
          <h1 className={styles.title}>{story.title}</h1>
          <p className={styles.author}>by {story.author}</p>

          {/* Optional summary box. */}
          {story.summary && (
            <div className={styles.summaryBox}>
              <p className={styles.summaryLabel}>Summary</p>
              <p className={styles.summaryText}>{story.summary}</p>
            </div>
          )}

          {/* Optional score section when available. */}
          {typeof story.score === "number" && (
            <div className={styles.scoreBox}>
              <div className={styles.scoreLeft}>
                <p className={styles.scoreTitle}>Relevance Score</p>
                <p className={styles.scoreValue}>{story.score.toFixed(2)}</p>
              </div>
              <div className={styles.scoreBarWrap}>
                <div className={styles.scoreTrack}>
                  <div
                    className={styles.scoreFill}
                    style={{ width: `${Math.min(story.score, 100)}%` }}
                  />
                </div>
                <p className={styles.scoreHint}>
                  out of 100 â€” higher means more semantically similar to your
                  query
                </p>
              </div>
            </div>
          )}
        </header>

        {/* Decorative divider between header and content. */}
        <div className={styles.divider}>
          <span>â§</span>
        </div>

        {/* Render story content with paragraph spacing. */}
        <main className={styles.content}>{renderContent(story.content)}</main>
      </div>
    </div>
  );
}
