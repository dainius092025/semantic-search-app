import { useEffect, useRef, useState } from "react";
import { useLocation } from "react-router-dom";
import SearchBar from "../components/SearchBar";
import ResultsGrid from "../components/ResultsGrid";
import StoryModal from "../components/StoryModal";
import { useSearch } from "../hooks/useSearch";
import { getAllStories } from "../api/stories";
import styles from "./SearchPage.module.css";

export default function SearchPage() {
  const location = useLocation();
  const { results, loading, error, hasSearched, lastQuery, search } = useSearch();
  const resultsRef = useRef(null);
  const [selectedStory, setSelectedStory] = useState(null);
  const [genreFilter, setGenreFilter] = useState("all");
  const [showScrollTop, setShowScrollTop] = useState(false);
  const [darkStoryTheme, setDarkStoryTheme] = useState(false);

  useEffect(() => {
    if (location.state?.initialSearch) {
      setGenreFilter("all");
      search(location.state.initialSearch);
    }
  }, [location.state?.initialSearch, search]);

  useEffect(() => {
    if (hasSearched && !loading && resultsRef.current) {
      resultsRef.current.scrollIntoView({ behavior: "smooth", block: "start" });
    }
  }, [hasSearched, loading]);

  useEffect(() => {
    const handleScroll = () => {
      setShowScrollTop(window.scrollY > 320);
    };

    handleScroll();
    window.addEventListener("scroll", handleScroll);

    return () => {
      window.removeEventListener("scroll", handleScroll);
    };
  }, []);

  function handleSearch(query) {
    setGenreFilter("all");
    search(query);
  }

  function handleGenreClick(genre) {
    if (!genre) return;
    setGenreFilter(genre);
    search(genre);
    setSelectedStory(null);
  }

  async function handleSurpriseClick() {
    try {
      let pool = Array.isArray(results) && results.length ? results : null;
      if (!pool) {
        pool = await getAllStories();
      }

      if (!pool || pool.length === 0) return;

      const rand = pool[Math.floor(Math.random() * pool.length)];
      setSelectedStory(rand);
    } catch (err) {}
  }

  function handleScrollTop() {
    window.scrollTo({ top: 0, behavior: "smooth" });
  }

  return (
    <div className={styles.pageWithImage}>
      <div className={styles.contentArea}>
        <div className={styles.heroWrap}>
          <SearchBar
            onSearch={handleSearch}
            loading={loading}
            darkStoryTheme={darkStoryTheme}
            onToggleStoryTheme={() => setDarkStoryTheme((value) => !value)}
          />
        </div>

      {error && (
        <div className={styles.error}>
          <strong>Error:</strong> {error}
        </div>
      )}

     {loading && (
        <div className={styles.loadingWrap}>
          <div className={styles.loadingDots}>
            <span /><span /><span />
          </div>
          <p>Searching the collection...</p>
        </div>
      )}

      <div ref={resultsRef}>
        {!loading && hasSearched && (
          <ResultsGrid
            results={results}
            query={lastQuery}
            onStoryClick={setSelectedStory}
            genreFilter={genreFilter}
            onGenreChange={setGenreFilter}
            darkStoryTheme={darkStoryTheme}
          />
        )}
      </div>

      {selectedStory && (
        <StoryModal
          story={selectedStory}
          onClose={() => setSelectedStory(null)}
          onGenreClick={handleGenreClick}
          darkStoryTheme={darkStoryTheme}
        />
      )}
      </div>
      <div className={styles.sideImage}>
        <span className={styles.cabinetCta}>
         Feeling curious? Tap here.
          <span className={styles.cabinetArrow} />
        </span>
        <a
          className={styles.sideImageLink}
          aria-label="Open a surprise tale"
          href="#"
          onClick={(e) => {
            e.preventDefault();
            handleSurpriseClick();
          }}
          onKeyDown={(e) => {
            if (e.key === "Enter" || e.key === " ") {
              e.preventDefault();
              handleSurpriseClick();
            }
          }}
        >
          <img className={styles.sideImageAsset} src="/bilde.png" alt="Bookshelf" />
        </a>
      </div>
      {showScrollTop && (
        <button
          type="button"
          className={styles.scrollTopButton}
          onClick={handleScrollTop}
          aria-label="Back to top"
        >
          Top
        </button>
      )}
    </div>
  );
}

