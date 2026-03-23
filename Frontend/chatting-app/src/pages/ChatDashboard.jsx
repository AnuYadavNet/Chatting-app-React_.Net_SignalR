import React, { useState } from "react";
import ChatPanel from "../components/ChatPanel";
import { useNavigate } from "react-router-dom";

export default function ChatDashboard() {
  const navigate = useNavigate();
  const username = localStorage.getItem("username");
  const [peerId, setPeerId] = useState("");
  const [activePeer, setActivePeer] = useState("");

  const handleLogout = () => {
    localStorage.removeItem("jwtToken");
    localStorage.removeItem("username");
    navigate("/login");
  };

  // Redirect to login if unauthenticated
  if (!username) {
    window.location.href = "/login";
    return null;
  }

  return (
    <div className="app-root">
      {/* Global Header */}
      <header className="app-header">
        <div className="header-brand">
          <span className="header-logo">⚡</span>
          <h1 className="header-title">ChattingApp</h1>
          <span className="header-tagline">Welcome, {username}!</span>
        </div>
        <div>
          <button
            onClick={handleLogout}
            style={{ background: '#ef4444', color: 'white', padding: '0.5rem 1rem', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold' }}
          >
            Logout
          </button>
        </div>
      </header>

      {/* Main Content Area */}
      <main className="chat-arena" style={{ flexDirection: 'column', padding: '2rem', display: 'flex', alignItems: 'center' }}>
        {!activePeer ? (
          <div style={{ background: '#1e293b', padding: '2rem', borderRadius: '12px', width: '100%', maxWidth: '400px', textAlign: 'center', marginTop: '4rem', color: 'white', boxShadow: '0 10px 15px -3px rgba(0,0,0,0.1)' }}>
            <h2 style={{ marginBottom: '1.5rem', color: '#f8fafc' }}>Start a Conversation</h2>
            <p style={{ color: '#94a3b8', fontSize: '0.875rem', marginBottom: '1.5rem' }}>Enter the username of the person you'd like to chat with.</p>
            <input
              value={peerId}
              onChange={e => setPeerId(e.target.value)}
              placeholder="Friend's username"
              style={{ width: '100%', padding: '0.875rem', margin: '0 0 1rem 0', borderRadius: '6px', border: '1px solid #334155', background: '#0f172a', color: 'white', outline: 'none', boxSizing: 'border-box' }}
            />
            <button
              onClick={() => { if (peerId.trim()) setActivePeer(peerId.trim()) }}
              style={{ padding: '0.875rem', background: '#0ea5e9', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', width: '100%', fontSize: '1rem', fontWeight: 'bold', transition: 'background 0.2s' }}
              onMouseOver={(e) => e.target.style.background = '#0284c7'}
              onMouseOut={(e) => e.target.style.background = '#0ea5e9'}
            >
              Start Chat
            </button>
          </div>
        ) : (
          <div style={{ display: 'flex', flexDirection: 'column', height: '100%', maxWidth: '800px', margin: '0 auto', width: '100%' }}>
            <div style={{ marginBottom: '1rem', display: 'flex', alignItems: 'center' }}>
              <button
                onClick={() => setActivePeer("")}
                style={{ background: '#334155', border: 'none', color: '#f8fafc', cursor: 'pointer', fontSize: '0.875rem', padding: '0.5rem 1rem', borderRadius: '44px', fontWeight: 'bold' }}
              >
                ← Back to Dashboard
              </button>
            </div>
            <div className="panel-wrapper" style={{ flex: 1, minHeight: 0, width: '100%' }}>
              <ChatPanel
                userId={username}
                peerId={activePeer}
                displayName={username}
                accentColor="#0ea5e9"
                side="left"
              />
            </div>
          </div>
        )}
      </main>
    </div>
  );
}
