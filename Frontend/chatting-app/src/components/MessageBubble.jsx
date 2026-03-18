// src/components/MessageBubble.jsx
// ─────────────────────────────────────────────────────────────────────────────
// Renders a single chat message bubble.
// Aligns right for "my" messages, left for incoming messages.
// ─────────────────────────────────────────────────────────────────────────────

import React from "react";

/**
 * Formats a UTC ISO timestamp into a human-friendly local time string.
 * e.g. "10:35 AM" for today, or "Mar 17, 10:35 AM" for older messages.
 */
const formatTimestamp = (isoString) => {
  const date = new Date(isoString);
  const now = new Date();
  const isToday =
    date.getDate()     === now.getDate()   &&
    date.getMonth()    === now.getMonth()  &&
    date.getFullYear() === now.getFullYear();

  if (isToday) {
    return date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
  }
  return date.toLocaleDateString([], { month: "short", day: "numeric" }) +
    ", " + date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
};

/**
 * @param {object}  message      - MessageDto from the server
 * @param {boolean} isMine       - True if this message was sent by the current panel's user
 * @param {string}  accentColor  - Used for the "mine" bubble background
 */
const MessageBubble = ({ message, isMine, accentColor }) => {
  return (
    <div className={`message-row ${isMine ? "mine" : "theirs"}`}>
      {/* Sender initial avatar — shown only for incoming messages */}
      {!isMine && (
        <div className="bubble-avatar" title={message.senderId}>
          {message.senderId.charAt(0).toUpperCase()}
        </div>
      )}

      <div className="bubble-content">
        {/* Sender label — shown only for incoming messages */}
        {!isMine && (
          <span className="bubble-sender">{message.senderId}</span>
        )}

        <div
          className={`bubble ${isMine ? "bubble-mine" : "bubble-theirs"}`}
          style={isMine ? { background: accentColor } : {}}
        >
          <p className="bubble-text">{message.messageText}</p>
        </div>

        <span className="bubble-time" title={new Date(message.timestamp).toLocaleString()}>
          {formatTimestamp(message.timestamp)}
          {isMine && (
            <span className="bubble-checkmarks" aria-label="Sent">
              {/* Double-checkmark delivery indicator */}
              ✓✓
            </span>
          )}
        </span>
      </div>
    </div>
  );
};

export default MessageBubble;
