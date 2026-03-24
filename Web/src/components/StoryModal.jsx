import { useEffect, useState } from "react";
import { getStoryById } from "../api/stories";
import styles from "./StoryModal.module.css";

export default function StoryModal({ story, onClose, onGenreClick }) {
  const [storyData, setStoryData] = useState(story);
  const [loading, setLoading] = useState(!story);
  const [error, setError] = useState(null);

  useEffect(() => {
    // Fetch full content only when caller passed a partial story payload.
    if (!story || !story.content) {
      getStoryById(story?.id)
        .then(setStoryData)
        .catch((err) => setError(err.message))
        .finally(() => setLoading(false));
    }
  }, [story]);

  const handleBackdropClick = (e) => {
    // Close only when clicking outside the modal content.
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
            <p>Loading storyâ€¦</p>
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
            <span className={styles.notFoundIcon}></span>
            <h2>Story not found</h2>
            <p>{error || "This story isn't available."}</p>
          </div>
        </div>
      </div>
    );
  }

  const { title, author, publishedYear, genre } = storyData;

  return (
    <div className={styles.backdrop} onClick={handleBackdropClick}>
      <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
        <button className={styles.closeBtn} onClick={onClose}>
          <svg
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
          >
            <line x1="18" y1="6" x2="6" y2="18" />
            <line x1="6" y1="6" x2="18" y2="18" />
          </svg>
        </button>

        <div className={styles.container}>
          <header className={styles.header}>
            <div className={styles.badges}>
              <span className={styles.yearBadge}>{publishedYear}</span>
            </div>
            <h2 className={styles.title}>{title}</h2>
            <p className={styles.author}>by {author}</p>
            <button
              type="button"
              className={styles.genreBadge}
              onClick={() => onGenreClick?.(genre)}
            >
              {genre}
            </button>
          </header>

          <main className={styles.content}>
            {storyData.content ? (
              // Preserve intentional paragraph breaks from backend text.
              storyData.content
                .split("\n")
                .map((para, i) =>
                  para.trim() ? <p key={i}>{para}</p> : <br key={i} />,
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