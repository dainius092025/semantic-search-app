import { Routes, Route } from "react-router-dom";
import Navbar from "./components/Navbar";
import SearchPage from "./pages/SearchPage";
import StoryDetailPage from "./pages/StoryDetailPage";

export default function App() {
  return (
    <>
      <Navbar />
      <Routes>
        <Route path="/" element={<SearchPage />} />
        <Route path="/story/:id" element={<StoryDetailPage />} />
      </Routes>
    </>
  );
}

