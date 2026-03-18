// src/App.jsx
// ─────────────────────────────────────────────────────────────────────────────
// Root component — renders the split-screen two-user chat layout.
//
// Layout:
//   Left  panel → User A (Anu)  — Teal accent
//   Right panel → User B (Pooja)    — Indigo accent
//   Divider     → Centered branding strip
// ─────────────────────────────────────────────────────────────────────────────

import React from "react";
import ChatPanel from "./components/ChatPanel";
import "./App.css";

const USER_A = { id: "Anu",  displayName: "Anu",  accent: "#0ea5e9" }; // Sky blue
const USER_B = { id: "Pooja",    displayName: "Pooja",    accent: "#8b5cf6" }; // Violet

function App() {
  return (
    <div className="app-root">
      {/* ── Global Header ── */}
      <header className="app-header">
        <div className="header-brand">
          <span className="header-logo">⚡</span>
          <h1 className="header-title">ChattingApp</h1>
          <span className="header-tagline">Real-time · Powered by SignalR</span>
        </div>
        <div className="header-tech-badges">
          <span className="tech-badge">.NET 8</span>
          <span className="tech-badge">SignalR</span>
          <span className="tech-badge">React</span>
          <span className="tech-badge">SQL Server</span>
        </div>
      </header>

      {/* ── Split-Screen Chat Area ── */}
      <main className="chat-arena">
        {/* Left: User A */}
        <div className="panel-wrapper panel-left">
          <ChatPanel
            userId={USER_A.id}
            peerId={USER_B.id}
            displayName={USER_A.displayName}
            accentColor={USER_A.accent}
            side="left"
          />
        </div>

        {/* Centre Divider */}
        <div className="arena-divider">
          <div className="divider-line" />
          <div className="divider-badge">VS</div>
          <div className="divider-line" />
        </div>

        {/* Right: User B */}
        <div className="panel-wrapper panel-right">
          <ChatPanel
            userId={USER_B.id}
            peerId={USER_A.id}
            displayName={USER_B.displayName}
            accentColor={USER_B.accent}
            side="right"
          />
        </div>
      </main>

      {/* ── Footer ── */}
      <footer className="app-footer">
        <span>ChattingApp17Mar2026 · Messages auto-delete after 12 hours</span>
      </footer>
    </div>
  );
}

export default App;
