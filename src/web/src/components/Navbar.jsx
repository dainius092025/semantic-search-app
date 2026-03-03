import { Link, useNavigate } from "react-router-dom";
import { useState } from "react";
import styles from "./Navbar.module.css";

export default function Navbar() {
  const navigate = useNavigate();
  const [searchQuery, setSearchQuery] = useState("");

  const handleSearch = () => {
    if (searchQuery.trim()) {
      navigate("/", { state: { initialSearch: searchQuery.trim() } });
      setSearchQuery("");
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter") {
      handleSearch();
    }
  };

  return (
    <div></div>
  );
}
