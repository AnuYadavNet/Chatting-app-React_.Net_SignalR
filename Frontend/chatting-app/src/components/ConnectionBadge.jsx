// src/components/ConnectionBadge.jsx
// ─────────────────────────────────────────────────────────────────────────────
// Small pill-shaped badge that reflects the current SignalR connection state.
// ─────────────────────────────────────────────────────────────────────────────

import React from "react";

const STATE_CONFIG = {
  Connected: {
    label: "Live",
    color: "#22c55e",
    pulse: true,
  },
  Reconnecting: {
    label: "Reconnecting…",
    color: "#f59e0b",
    pulse: false,
  },
  Disconnected: {
    label: "Offline",
    color: "#ef4444",
    pulse: false,
  },
};

/**
 * @param {string} state - "Connected" | "Reconnecting" | "Disconnected"
 */
const ConnectionBadge = ({ state }) => {
  const config = STATE_CONFIG[state] || STATE_CONFIG.Disconnected;

  return (
    <div className="connection-badge" title={`SignalR: ${state}`}>
      <span
        className={`badge-dot ${config.pulse ? "badge-dot--pulse" : ""}`}
        style={{ background: config.color }}
      />
      <span className="badge-label" style={{ color: config.color }}>
        {config.label}
      </span>
    </div>
  );
};

export default ConnectionBadge;
