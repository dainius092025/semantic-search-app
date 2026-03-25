import styles from "./StoryCard.module.css";
import { normalizeScore, scoreLabel } from "../utils/score";

const SCORE_BAR_COLOR_HIGH = "#22c55e";
const SCORE_BAR_COLOR_LOW = "#c8860a";

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

function getGenreStyle(genre) {
  return (
    GENRE_COLORS[genre] || {
      bg: "var(--oatmilk)",
      color: "var(--text-muted)",
    }
  );
}

function scoreInfo(score) {
  const pct = Math.max(0, Math.min(Math.round(score), 100));
  const bar = score >= 55 ? SCORE_BAR_COLOR_HIGH : SCORE_BAR_COLOR_LOW;
  return { label: scoreLabel(score), bar, pct };
}

export default function StoryCard({ story, index, onClick }) {
  const { title, author, genre, publishedYear, summary, score } = story;
  const genreStyle = getGenreStyle(genre);
  const adjustedScore = normalizeScore(score);
  const hasScore = typeof adjustedScore === "number";
  const scoreMeta = hasScore ? scoreInfo(adjustedScore) : null;
  const handleKeyDown = (e) => {
    if (e.key === "Enter" || e.key === " ") {
      e.preventDefault();
      onClick?.();
    }
  };

  return (
    <article
      className={styles.card}
      style={{ animationDelay: `${index * 55}ms` }}
      onClick={onClick}
      onKeyDown={handleKeyDown}
      role="button"
      tabIndex={0}
    >
      <div className={styles.top}>
        <span
          className={styles.genreBadge}
          style={{ background: genreStyle.bg, color: genreStyle.color }}
        >
          {genre}
        </span>
        <span className={styles.year}>{publishedYear}</span>
      </div>

      <h2 className={styles.title}>{title}</h2>
      <p className={styles.author}>by {author}</p>

      <p className={styles.excerpt}>{summary}</p>

      {hasScore && (
        <div className={styles.scoreWrap}>
          <div className={styles.scoreTrack}>
            <div
              className={styles.scoreFill}
              style={{ width: `${scoreMeta.pct}%`, background: scoreMeta.bar }}
            />
          </div>
          <div className={styles.scoreMeta}>
            <span className={styles.scoreLabel}>{scoreMeta.label}</span>
          </div>
        </div>
      )}

      <div className={styles.footer}>
        <span className={styles.readMore}>
          Read story
          <svg
            width="14"
            height="14"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
          >
            <path d="M5 12h14M12 5l7 7-7 7" />
          </svg>
        </span>
        {hasScore && (
          <span className={styles.scoreNum} style={{ color: scoreMeta.bar }}>
            {adjustedScore.toFixed(1)}%
          </span>
        )}
      </div>
    </article>
  );
}

