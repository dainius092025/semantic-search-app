// React import enables JSX runtime features in this entry file.
import React from "react";
// ReactDOM client API mounts the React app into the DOM.
import ReactDOM from "react-dom/client";
// BrowserRouter provides HTML5 history-based routing.
import { BrowserRouter } from "react-router-dom";
// Root application component.
import App from "./App.jsx";
// Global styles for the entire app.
import "./index.css";

// Create a root bound to the #root element and render the app tree.
ReactDOM.createRoot(document.getElementById("root")).render(
  // StrictMode highlights potential problems in development.
  <React.StrictMode>
    <BrowserRouter>
      {/* App contains the main layout and route table. */}
      <App />
    </BrowserRouter>
  </React.StrictMode>,
);
