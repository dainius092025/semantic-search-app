// React hooks for lifecycle, refs, and local state.
import { useEffect, useRef, useState } from "react";
// Router hook to access navigation state.
import { useLocation } from "react-router-dom";
// Search input component.
import SearchBar from "../components/SearchBar";
// Grid that renders search results and filtering UI.
import ResultsGrid from "../components/ResultsGrid";
// Modal that shows a selected story in detail.
import StoryModal from "../components/StoryModal";
// Custom hook that manages search logic and state.
import { useSearch } from "../hooks/useSearch";
// API helper for fetching all stories when needed.
import { getAllStories } from "../api/stories";
// CSS module for this page's styles.
import styles from "./SearchPage.module.css";

// Search page component handles query, filters, results, and modal.
export default function SearchPage() {
  // Access router location to read optional navigation state.
  const location = useLocation();
  // Destructure search state and actions from the custom hook.
  const {
    results,
    loading,
    error,
    hasSearched,
    lastQuery,
    search,
  } = useSearch();

  // Ref to the results container to support scroll-into-view.
  const resultsRef = useRef(null);
  // Selected story drives the modal visibility/content.
  const [selectedStory, setSelectedStory] = useState(null);
  // Active genre filter for results grid.
  const [genreFilter, setGenreFilter] = useState("all");
  // Toggles visibility of the "back to top" button.
  const [showTopButton, setShowTopButton] = useState(false);

  // If navigation provides a pre-filled query, run it on page load.
  useEffect(() => {
    // When a link passes initialSearch, auto-run it.
    if (location.state?.initialSearch) {
      // Reset the genre filter for a fresh search.
      setGenreFilter("all");
      // Run the search.
      search(location.state.initialSearch);
    }
    // Only re-run when the initial search value changes.
  }, [location.state?.initialSearch]);

  // Scroll to the results section after each completed search.
  useEffect(() => {
    // Wait until we have results and a mounted ref.
    if (hasSearched && !loading && resultsRef.current) {
      // Smoothly scroll the results area into view.
      resultsRef.current.scrollIntoView({ behavior: "smooth", block: "start" });
    }
    // Re-evaluate on search completion or loading state.
  }, [hasSearched, loading]);

  // Track page scroll to toggle the back-to-top button.
  useEffect(() => {
    // Update visibility based on current scroll position.
    const handleScroll = () => {
      setShowTopButton(window.scrollY > 240);
    };

    // Initialize the button state on mount.
    handleScroll();
    // Listen to scroll events with passive option for performance.
    window.addEventListener("scroll", handleScroll, { passive: true });
    // Clean up the listener on unmount.
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  // Click handler to apply a genre from the modal or grid.
  function handleGenreClick(genre) {
    // Ignore empty selections.
    if (!genre) return;
    // Update the active genre filter.
    setGenreFilter(genre);
    // Close the modal when a new genre is selected.
    setSelectedStory(null);
  }

  // Handler that runs a new search and resets filters.
  function handleSearch(query) {
    // Clear any existing genre filter when searching.
    setGenreFilter("all");
    // Execute the search.
    search(query);
  }

  // Picks a random story either from results or from the full catalog.
  async function handleCabinetClick() {
    // Avoid concurrent actions while loading.
    if (loading) return;
    try {
      // Prefer current results as the selection pool.
      let pool = Array.isArray(results) && results.length ? results : null;
      if (!pool) {
        // Fall back to all stories if no results are present.
        pool = await getAllStories();
      }

      // If we still have no data, abort.
      if (!pool || pool.length === 0) return;

      // Choose a random story and open the modal.
      const rand = pool[Math.floor(Math.random() * pool.length)];
      setSelectedStory(rand);
    } catch {
      // Ignore random-pick errors silently.
    }
  }

  // Smoothly scroll to the top of the page.
  function handleScrollToTop() {
    window.scrollTo({ top: 0, behavior: "smooth" });
  }

  // Page layout and conditional UI rendering.
  return (
    <div className={styles.pageWithImage}>
      <div className={styles.contentArea}>
        <div className={styles.heroWrap}>
          {/* SearchBar controls the query. */}
          <SearchBar
            onSearch={handleSearch}
            loading={loading}
          />
        </div>

        {/* Error message from the search hook. */}
        {error && (
          <div className={styles.error}>
            <strong>Error:</strong> {error}
          </div>
        )}

        {/* Loading indicator shown while a search is in progress. */}
        {loading && (
          <div className={styles.loadingWrap}>
            <div className={styles.loadingDots}>
              <span />
              <span />
              <span />
            </div>
            <p>Searching the collection...</p>
          </div>
        )}

        {/* Results container to allow scroll-into-view. */}
        <div ref={resultsRef}>
          {/* Render results only after a search completes. */}
          {!loading && hasSearched && (
            <ResultsGrid
              results={results}
              query={lastQuery}
              onStoryClick={setSelectedStory}
              genreFilter={genreFilter}
              onGenreChange={setGenreFilter}
            />
          )}
        </div>

        {/* Modal with story details when a story is selected. */}
        {selectedStory && (
          <StoryModal
            story={selectedStory}
            onClose={() => setSelectedStory(null)}
            onGenreClick={handleGenreClick}
          />
        )}
        {/* Floating back-to-top button. */}
        <button
          type="button"
          className={`${styles.toTop} ${showTopButton ? styles.toTopVisible : ""}`}
          onClick={handleScrollToTop}
          aria-label="Back to top"
        >
          &#129093;
        </button>
      </div>

      {/* Side image area with an interactive "bookshelf" hotspot. */}
      <div className={styles.sideImage} aria-hidden="true">
        {/* Call-to-action text for the surprise selection. */}
        <span className={styles.cabinetCta}>
          Tap the Bookshelf for a Surprise Tale!
        </span>
        {/* Invisible button aligned to the bookshelf image. */}
        <button
          type="button"
          className={styles.bookshelfHotspot}
          onClick={handleCabinetClick}
          aria-label="Tap the Bookshelf for a Surprise Tale!"
        />
      </div>
    </div>
  );
}
