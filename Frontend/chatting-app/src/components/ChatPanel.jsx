// src/components/ChatPanel.jsx
// ─────────────────────────────────────────────────────────────────────────────
// Renders a single user's chat panel: message list + input bar.
// Completely reusable — used for both User A and User B panels.
// ─────────────────────────────────────────────────────────────────────────────

import React, { useState, useEffect, useRef } from "react";
import useChatSignalR from "../hooks/useChatSignalR";
import MessageBubble from "./MessageBubble";
import ConnectionBadge from "./ConnectionBadge";

/**
 * @param {string}  userId       - This panel's user identity (e.g. "UserA")
 * @param {string}  peerId       - The other user's identity (e.g. "UserB")
 * @param {string}  displayName  - Friendly label shown in the panel header
 * @param {string}  accentColor  - CSS hex/var for panel's accent colour
 * @param {string}  side         - "left" | "right" — controls bubble alignment
 */
const ChatPanel = ({ userId, peerId, displayName, accentColor, side }) => {
  const [inputValue, setInputValue] = useState("");
  const messagesEndRef = useRef(null);

  const { messages, connectionState, error, sendMessage } = useChatSignalR(userId, peerId);

  // ── Auto-scroll to bottom whenever messages change ──────────────────────
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  // ── Handle send (keyboard + button) ─────────────────────────────────────
  const handleSend = async () => {
    if (!inputValue.trim()) return;
    await sendMessage(inputValue);
    setInputValue("");
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  const isConnected = connectionState === "Connected";

  return (
    <div className="chat-panel" style={{ "--accent": accentColor }}>
      {/* ── Panel Header ── */}
      <div className="panel-header" style={{ background: accentColor }}>
        <div className="panel-header-left">
          <div className="avatar" style={{ background: accentColor }}>
            {displayName.charAt(0).toUpperCase()}
          </div>
          <div className="panel-header-info">
            <span className="panel-title">{displayName}</span>
            <span className="panel-subtitle">Chatting with {peerId}</span>
          </div>
        </div>
        <ConnectionBadge state={connectionState} />
      </div>

      {/* ── Error Banner ── */}
      {error && (
        <div className="error-banner" role="alert">
          <span className="error-icon">⚠</span>
          <span>{error}</span>
        </div>
      )}

      {/* ── Message List ── */}
      <div className="message-list" aria-live="polite" aria-label={`${displayName} chat messages`}>
        {messages.length === 0 ? (
          <div className="empty-state">
            <div className="empty-icon">💬</div>
            <p>No messages yet.</p>
            <p className="empty-hint">Say hello!</p>
          </div>
        ) : (
          messages.map((msg) => (
            <MessageBubble
              key={msg.messageId}
              message={msg}
              isMine={msg.senderId === userId}
              accentColor={accentColor}
            />
          ))
        )}
        {/* Invisible element to scroll to */}
        <div ref={messagesEndRef} />
      </div>

      {/* ── Input Bar ── */}
      <div className="input-bar">
        <textarea
          className="message-input"
          placeholder={isConnected ? `Message as ${displayName}…` : "Connecting…"}
          value={inputValue}
          onChange={(e) => setInputValue(e.target.value)}
          onKeyDown={handleKeyDown}
          disabled={!isConnected}
          maxLength={2000}
          rows={1}
          aria-label={`Type a message as ${displayName}`}
        />
        <button
          className="send-button"
          onClick={handleSend}
          disabled={!isConnected || !inputValue.trim()}
          style={{ background: accentColor }}
          aria-label="Send message"
          title="Send (Enter)"
        >
          <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M22 2L11 13" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
            <path d="M22 2L15 22L11 13L2 9L22 2Z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
          </svg>
        </button>
      </div>

      {/* ── Character counter ── */}
      {inputValue.length > 1800 && (
        <div className="char-counter" style={{ color: inputValue.length > 1950 ? "#ef4444" : "#f59e0b" }}>
          {2000 - inputValue.length} characters remaining
        </div>
      )}
    </div>
  );
};

export default ChatPanel;
