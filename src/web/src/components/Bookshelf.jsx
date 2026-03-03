import { useState } from "react";
import "./Bookshelf.css";

export default function Bookshelf() {
  const [story, setStory] = useState(null);

  const handleClick = async (storyId) => {
    const res = await fetch(`/api/random-story?story=${storyId}`);
    const data = await res.json();
    setStory(data.story);
  };

  return (
    <div className="shelf-wrapper">
      <div className="shelf-container">
        <img src="/bilde.png" className="shelf-image" alt="Bookshelf" />

   
        <div
          className="hotspot"
          style={{ top: "10%", left: "53%", width: "10%", height: "10%" }}
          onClick={() => handleClick("")}
        />

        <div
          className="hotspot"
          style={{ top: "10%", left: "63%", width: "10%", height: "10%" }}
          onClick={() => handleClick("")}
        />

        <div
          className="hotspot"
          style={{ top: "10%", left: "73%", width: "10%", height: "10%" }}
          onClick={() => handleClick("")}
        />

        <div
          className="hotspot"
          style={{ top: "10%", left: "83%", width: "10%", height: "10%" }}
          onClick={() => handleClick("")}
        />

     
        <div
          className="hotspot"
          style={{ top: "28%", left: "8%", width: "28%", height: "5%" }}
          onClick={() => handleClick("")}
        />

        <div
          className="hotspot"
          style={{ top: "33%", left: "8%", width: "28%", height: "5%" }}
          onClick={() => handleClick("")}
        />

        <div
          className="hotspot"
          style={{ top: "28%", left: "60%", width: "28%", height: "10%" }}
          onClick={() => handleClick("")}
        />

        {/* THIRD SHELF */}
        <div
          className="hotspot"
          style={{ top: "20%", left: "20%", width: "26%", height: "5%" }}
          onClick={() => handleClick("")}
        />

        <div
          className="hotspot"
          style={{ top: "52%", left: "20%", width: "34%", height: "5%" }}
          onClick={() => handleClick("")}
        />

        {/* FOURTH SHELF */}
        <div
          className="hotspot"
          style={{ top: "67%", left: "16%", width: "30%", height: "6%" }}
          onClick={() => handleClick("")}
        />

        <div
          className="hotspot"
          style={{ top: "73%", left: "16%", width: "30%", height: "6%" }}
          onClick={() => handleClick("")}
        />

        {story && (
          <div className="story-box">
            <h2>Story</h2>
            <p>{story}</p>
          </div>
        )}
      </div>  
    </div>
  );
}