import { useEffect, useState } from "react";
import { getStoryById } from "../api/stories";
import styles from "./StoryModal.module.css";

const GENRE_COLORS = {
  "Literary Fiction":    { bg: "#f0f4ff", color: "#3730a3" },
  "Science Fiction":     { bg: "#ecfdf5", color: "#065f46" },
  "Mystery":             { bg: "#fdf4ff", color: "#7e22ce" },
  "Fantasy":             { bg: "#fff7ed", color: "#c2410c" },
  "Contemporary Drama":  { bg: "#fef9c3", color: "#854d0e" },
  "Tech Thriller":       { bg: "#f0fdf4", color: "#166534" },
  "Magical Realism":     { bg: "#fdf2f8", color: "#9d174d" },
  "Historical Fiction":  { bg: "#fef3c7", color: "#92400e" },
  "Post-Apocalyptic":    { bg: "#f1f5f9", color: "#334155" },
  "Humor":               { bg: "#fef9c3", color: "#713f12" },
};

function getGenreStyle(genre) {
  return GENRE_COLORS[genre] || { bg: "var(--parchment-dark)", color: "var(--text-muted)" };
}

export default function StoryModal({ story, onClose }) {
  const [storyData, setStoryData] = useState(story);
  const [loading, setLoading] = useState(!story);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!story || !story.content) {
      getStoryById(story?.id)
        .then(setStoryData)
        .catch((err) => setError(err.message))
        .finally(() => setLoading(false));
    }
  }, [story]);

  const handleBackdropClick = (e) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  if (loading) {
    return (
      <div className={styles.backdrop} onClick={handleBackdropClick}>
        <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
          <div className={styles.center}>
            <div className={styles.spinner} />
            <p>Loading story…</p>
          </div>
        </div>
      </div>
    );
  }

  if (error || !storyData) {
    return (
      <div className={styles.backdrop} onClick={handleBackdropClick}>
        <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
          <div className={styles.center}>
            <span className={styles.notFoundIcon}>❧</span>
            <h2>Story not found</h2>
            <p>{error || "This story isn't available."}</p>
          </div>
        </div>
      </div>
    );
  }

  const { title, author, genre, publishedYear, summary, content, id } = storyData;
  const genreStyle = getGenreStyle(genre);

  return (
    <div className={styles.backdrop} onClick={handleBackdropClick}>
      <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
        <button className={styles.closeBtn} onClick={onClose}>
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <line x1="18" y1="6" x2="6" y2="18" />
            <line x1="6" y1="6" x2="18" y2="18" />
          </svg>
        </button>

        <div className={styles.container}>
          <main className={styles.content}>
            {content ? (
              content.split("\n").map((para, i) =>
                para.trim() ? <p key={i}>{para}</p> : <br key={i} />
              )
            ) : (
              <p className={styles.noContent}>Full content not available.</p>
            )}
          </main>
        </div>
      </div>
    </div>
  );
}
