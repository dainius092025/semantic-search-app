import { useEffect, useState } from "react";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import { getStoryById } from "../api/stories";
import { normalizeScore } from "../utils/score";
import styles from "./StoryDetailPage.module.css";

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
  return GENRE_COLORS[genre] || {
    bg: "var(--oatmilk)",
    color: "var(--text-muted)",
  };
}

function renderContent(content) {
  if (!content) {
    return <p className={styles.noContent}>Full content not available.</p>;
  }

  return content.split("\n").map((para, i) =>
    para.trim() ? <p key={i}>{para}</p> : <br key={i} />
  );
}

export default function StoryDetailPage() {
  const navigate = useNavigate();
  const { state } = useLocation();
  const { id } = useParams();

  const [story, setStory] = useState(state?.story || null);
  const [loading, setLoading] = useState(!state?.story);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!state?.story) {
      getStoryById(id)
        .then(setStory)
        .catch((err) => setError(err.message))
        .finally(() => setLoading(false));
    }
  }, [id, state?.story]);

  if (loading) {
    return (
      <div className={styles.center}>
        <div className={styles.spinner} />
        <p>Loading story...</p>
      </div>
    );
  }

  if (error || !story) {
    return (
      <div className={styles.notFound}>
        <span className={styles.notFoundIcon}>*</span>
        <h2>Story not found</h2>
        <p>{error || "This story isn't available."}</p>
        <button type="button" className={styles.backBtn} onClick={() => navigate("/")}>
          Back to Search
        </button>
      </div>
    );
  }

  const genreStyle = getGenreStyle(story.genre);
  const displayScore = normalizeScore(story.score);

  return (
    <div className={styles.page}>
      <div className={styles.container}>
        <button type="button" className={styles.back} onClick={() => navigate(-1)}>
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
            <span
              className={styles.genreBadge}
              style={{ background: genreStyle.bg, color: genreStyle.color }}
            >
              {story.genre}
            </span>
            <span className={styles.yearBadge}>{story.publishedYear}</span>
            <span className={styles.idBadge}>Story #{id}</span>
          </div>

          <h1 className={styles.title}>{story.title}</h1>
          <p className={styles.author}>by {story.author}</p>

          {story.summary && (
            <div className={styles.summaryBox}>
              <p className={styles.summaryLabel}>Summary</p>
              <p className={styles.summaryText}>{story.summary}</p>
            </div>
          )}

          {typeof displayScore === "number" && (
            <div className={styles.scoreBox}>
              <div className={styles.scoreLeft}>
                <p className={styles.scoreTitle}>Relevance Score</p>
                <p className={styles.scoreValue}>{displayScore.toFixed(1)}%</p>
              </div>
              <div className={styles.scoreBarWrap}>
                <div className={styles.scoreTrack}>
                  <div
                    className={styles.scoreFill}
                    style={{ width: `${Math.min(displayScore, 100)}%` }}
                  />
                </div>
                <p className={styles.scoreHint}>
                  Out of 100 - higher means more semantically similar to your
                  query.
                </p>
              </div>
            </div>
          )}
        </header>

        <div className={styles.divider}>
          <span>*</span>
        </div>

        <main className={styles.content}>{renderContent(story.content)}</main>
      </div>
    </div>
  );
}

