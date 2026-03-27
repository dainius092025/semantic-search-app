import { useEffect, useState } from "react";
import { getStoryById } from "../api/stories";
import styles from "./StoryModal.module.css";

export default function StoryModal({ story, onClose, onGenreClick, darkStoryTheme = false }) {
  const [storyData, setStoryData] = useState(story);
  const [loading, setLoading] = useState(!story?.content);
  const [error, setError] = useState(null);

  useEffect(() => {
    let cancelled = false;

    setStoryData(story ?? null);
    setError(null);
    setLoading(!story?.content);

    if (!story) {
      return () => {
        cancelled = true;
      };
    }

    if (story.content) {
      return () => {
        cancelled = true;
      };
    }

    getStoryById(story.id)
      .then((data) => {
        if (!cancelled) {
          setStoryData(data);
        }
      })
      .catch((err) => {
        if (!cancelled) {
          setError(err.message);
        }
      })
      .finally(() => {
        if (!cancelled) {
          setLoading(false);
        }
      });

    return () => {
      cancelled = true;
    };
  }, [story]);

  const handleBackdropClick = (e) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  if (loading) {
    return (
      <div className={styles.backdrop} onClick={handleBackdropClick}>
        <div
          className={`${styles.modal} ${darkStoryTheme ? styles.modalDark : ""}`}
          onClick={(e) => e.stopPropagation()}
        >
          <div className={styles.center}>
            <div className={styles.spinner} />
            <p>Loading story...</p>
          </div>
        </div>
      </div>
    );
  }

  if (error || !storyData) {
    return (
      <div className={styles.backdrop} onClick={handleBackdropClick}>
        <div
          className={`${styles.modal} ${darkStoryTheme ? styles.modalDark : ""}`}
          onClick={(e) => e.stopPropagation()}
        >
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
      <div
        className={`${styles.modal} ${darkStoryTheme ? styles.modalDark : ""}`}
        onClick={(e) => e.stopPropagation()}
      >
        <button type="button" className={styles.closeBtn} onClick={onClose}>
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

