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
  const { results, loading, error, hasSearched, lastQuery, search, mode, setMode } = useSearch();
  const resultsRef = useRef(null);
  const [selectedStory, setSelectedStory] = useState(null);

  useEffect(() => {
    if (location.state?.initialSearch) {
      search(location.state.initialSearch, mode);
    }
  }, [location.state?.initialSearch]);

  useEffect(() => {
    if (hasSearched && !loading && resultsRef.current) {
      resultsRef.current.scrollIntoView({ behavior: "smooth", block: "start" });
    }
  }, [hasSearched, loading]);

  // Hotspots configuration (percent numbers relative to the side image)
  const DEFAULT_HOTSPOTS = [
    { id: "h1", left: 12, top: 58 },
    { id: "h2", left: 36, top: 42 },
    { id: "h3", left: 60, top: 70 },
  ];

  // allow editing and persisting hotspots
  const sideRef = useRef(null);
  const [hotspots, setHotspots] = useState(() => {
    try {
      const raw = localStorage.getItem("hotspots_v1");
      return raw ? JSON.parse(raw) : DEFAULT_HOTSPOTS;
    } catch (e) {
      return DEFAULT_HOTSPOTS;
    }
  });
  const [isEditing, setIsEditing] = useState(false);
  const dragging = useRef(null);

  async function handleHotspotClick() {
    // Prefer current search results, otherwise fetch all stories
    try {
      let pool = Array.isArray(results) && results.length ? results : null;
      if (!pool) {
        pool = await getAllStories();
      }

      if (!pool || pool.length === 0) return;

      const rand = pool[Math.floor(Math.random() * pool.length)];
      setSelectedStory(rand);
    } catch (err) {
      // ignore errors silently for hotspot
    }
  }

  function startDrag(e, id) {
    if (!isEditing) return;
    e.preventDefault();
    dragging.current = { id };
    window.addEventListener("pointermove", onPointerMove);
    window.addEventListener("pointerup", endDrag);
  }

  function onPointerMove(e) {
    if (!dragging.current) return;
    const el = sideRef.current;
    if (!el) return;
    const rect = el.getBoundingClientRect();
    // clamp
    const x = Math.max(0, Math.min(1, (e.clientX - rect.left) / rect.width));
    const y = Math.max(0, Math.min(1, (e.clientY - rect.top) / rect.height));
    setHotspots((prev) => prev.map((h) => (h.id === dragging.current.id ? { ...h, left: Math.round(x * 100), top: Math.round(y * 100) } : h)));
  }

  function endDrag() {
    dragging.current = null;
    window.removeEventListener("pointermove", onPointerMove);
    window.removeEventListener("pointerup", endDrag);
  }

  function saveHotspots() {
    try {
      localStorage.setItem("hotspots_v1", JSON.stringify(hotspots));
      setIsEditing(false);
    } catch (e) {
      // ignore
    }
  }

  function resetHotspots() {
    setHotspots(DEFAULT_HOTSPOTS);
    localStorage.removeItem("hotspots_v1");
  }
  
  function addHotspotAtEvent(e) {
    if (!isEditing) return;
    // only respond when clicking the side container itself, not a control
    if (e.target !== sideRef.current) return;
    const rect = sideRef.current.getBoundingClientRect();
    const x = Math.max(0, Math.min(1, (e.clientX - rect.left) / rect.width));
    const y = Math.max(0, Math.min(1, (e.clientY - rect.top) / rect.height));
    const id = `h${Date.now().toString(36)}`;
    setHotspots((p) => [...p, { id, left: Math.round(x * 100), top: Math.round(y * 100) }]);
  }

  function removeHotspot(id) {
    setHotspots((p) => p.filter((h) => h.id !== id));
  }
  return (
    <div className={styles.pageWithImage}>
      <div className={styles.contentArea}>
        <div className={styles.heroWrap}>
          <SearchBar
            onSearch={search}
            loading={loading}
            mode={mode}
            onModeChange={setMode}
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
          <p>Searching the collection…</p>
        </div>
      )}

      <div ref={resultsRef}>
        {!loading && hasSearched && (
          <ResultsGrid results={results} query={lastQuery} onStoryClick={setSelectedStory} />
        )}
      </div>

      {selectedStory && (
        <StoryModal story={selectedStory} onClose={() => setSelectedStory(null)} />
      )}
      </div>
      <div className={styles.sideImage} ref={sideRef} onPointerDown={addHotspotAtEvent}>
        {!isEditing && (
          <button className={styles.editToggle} onClick={() => setIsEditing(true)} aria-label="Edit hotspots">Edit</button>
        )}
        {hotspots.map((h) => (
          <button
            key={h.id}
            className={`${styles.hotspot} ${isEditing ? styles.editing : ""}`}
            style={{ left: `${h.left}%`, top: `${h.top}%` }}
            onClick={handleHotspotClick}
            onPointerDown={(e) => startDrag(e, h.id)}
            aria-label={`Hotspot ${h.id}`}
            title={isEditing ? `${h.left}%, ${h.top}%` : ""}
          />
        ))}

        {isEditing && (
          <div className={styles.hotspotEditor}>
            <div className={styles.editorRow}>
              <button onClick={() => setIsEditing(false)}>Done</button>
              <button onClick={saveHotspots}>Save</button>
              <button onClick={resetHotspots}>Reset</button>
            </div>
            <div className={styles.editorList}>
              {hotspots.map((h) => (
                <div key={h.id} className={styles.editorItem}>
                  <strong>{h.id}</strong>
                  <label>
                    L:
                    <input type="number" value={h.left} onChange={(e) => setHotspots((p) => p.map(x => x.id===h.id?{...x,left:Math.max(0,Math.min(100,Number(e.target.value)||0))}:x))} />
                  </label>
                  <label>
                    T:
                    <input type="number" value={h.top} onChange={(e) => setHotspots((p) => p.map(x => x.id===h.id?{...x,top:Math.max(0,Math.min(100,Number(e.target.value)||0))}:x))} />
                  </label>
                  <button className={styles.removeHotspot} onClick={() => removeHotspot(h.id)}>Remove</button>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}