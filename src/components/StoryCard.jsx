import styles from "./StoryCard.module.css";

const GENRE_COLORS = {
  "Literary Fiction":   { bg: "#f0f4ff", color: "#3730a3" },
  "Science Fiction":    { bg: "#ecfdf5", color: "#065f46" },
  "Mystery":            { bg: "#fdf4ff", color: "#7e22ce" },
  "Fantasy":            { bg: "#fff7ed", color: "#c2410c" },
  "Contemporary Drama": { bg: "#fef9c3", color: "#854d0e" },
  "Tech Thriller":      { bg: "#f0fdf4", color: "#166534" },
  "Magical Realism":    { bg: "#fdf2f8", color: "#9d174d" },
  "Historical Fiction": { bg: "#fef3c7", color: "#92400e" },
  "Post-Apocalyptic":   { bg: "#f1f5f9", color: "#334155" },
  "Humor":              { bg: "#fef9c3", color: "#713f12" },
};

function getGenreStyle(genre) {
  return GENRE_COLORS[genre] || { bg: "var(--parchment-dark)", color: "var(--text-muted)" };
}

// Score is 0–100 float from the API (adjusted by +10 for display)
function scoreInfo(score) {
  if (score >= 60) return { label: "Strong match",  bar: "#22c55e", pct: Math.round(score, 100) };
  if (score >= 45) return { label: "Good match",    bar: "#eab308", pct: Math.round(score, 100) };
  if (score >= 30) return { label: "Partial match", bar: "#f97316", pct: Math.round(score, 100) };
  return               { label: "Weak match",    bar: "#9ca3af", pct: Math.round(score, 100) };
}

export default function StoryCard({ story, index, onClick }) {
  const { id, title, author, genre, publishedYear, summary, score } = story;
  const genreStyle = getGenreStyle(genre);
  const hasScore = typeof score === "number";
  const adjustedScore = score + 10;
  const si = hasScore ? scoreInfo(adjustedScore) : null;

  return (
    <article
      className={styles.card}
      style={{ animationDelay: `${index * 55}ms` }}
      onClick={onClick}
    >
      <div className={styles.top}>
        <span className={styles.genreBadge} style={{ background: genreStyle.bg, color: genreStyle.color }}>
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
            <div className={styles.scoreFill} style={{ width: `${si.pct}%`, background: si.bar }} />
          </div>
          <div className={styles.scoreMeta}>
            <span className={styles.scoreLabel}>{si.label}</span>
            
          </div>
        </div>
      )}

      <div className={styles.footer}>
        <span className={styles.readMore}>
          Read story
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <path d="M5 12h14M12 5l7 7-7 7"/>
          </svg>
        </span>
       {hasScore && (
          <span className={styles.scoreNum} style={{ color: si.bar }}>
            {adjustedScore.toFixed(1)}%
          </span>
        )}
       
      </div>
    </article>
  );
}
