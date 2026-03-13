// Route components for client-side navigation.
import { Routes, Route } from "react-router-dom";
// Persistent top navigation bar.
import Navbar from "./components/Navbar";
// Search page shown at the root path.
import SearchPage from "./pages/SearchPage";
// Detail page for a single story.
import StoryDetailPage from "./pages/StoryDetailPage";

// Root app component defines shared layout and routes.
export default function App() {
  return (
    <>
      {/* Global navbar shown on every page. */}
      <Navbar />
      {/* Route switch maps paths to page components. */}
      <Routes>
        {/* Home/search route. */}
        <Route path="/" element={<SearchPage />} />
        {/* Story detail route with dynamic id. */}
        <Route path="/story/:id" element={<StoryDetailPage />} />
      </Routes>
    </>
  );
}
