# ⚛️ Frontend Knowledge Base
### React · SignalR Client · Real-time Chat UI
---

## 🧱 Overview
The frontend is a **React-based real-time chat application** that communicates with the backend using **SignalR**.
It is:
* Event-driven
* Real-time (WebSocket-based)
* Hook-driven architecture
---

## 🏗️ Architecture
UI Components → Hooks → SignalR Service → Backend Hub
                                     ↓
                               Real-time events
                                     ↓
UI updates instantly
---

## 📂 Folder Structure
Frontend/
└── chatting-app/
    ├── public/
    │   └── index.html
    │
    ├── src/
    │   ├── components/
    │   │   ├── ChatPanel.jsx
    │   │   ├── MessageBubble.jsx
    │   │   └── ConnectionBadge.jsx
    │   │
    │   ├── pages/
    │   │   ├── ChatDashboard.jsx
    │   │   ├── Login.jsx
    │   │   └── Register.jsx
    │   │
    │   ├── hooks/
    │   │   └── useChatSignalR.js
    │   │
    │   ├── services/
    │   │   ├── api.js
    │   │   └── signalRService.js
    │   │
    │   ├── App.jsx
    │   └── index.js
---

## 🔄 Core Flow
### 1. Sending Message
User Action (Enter / Click Send)
        ↓
ChatPanel.jsx → sendMessage(text)
        ↓
useChatSignalR → connection.invoke("SendMessage", dto)
        ↓
Backend Hub
---

### 2. Receiving Message
SignalR Event: "ReceiveMessage"
        ↓
connection.on("ReceiveMessage")
        ↓
setMessages(...)
        ↓
React re-render
---

## 🔌 SignalR Layer
### useChatSignalR.js
Responsible for:
* Creating connection
* Starting connection
* Reconnection handling
* Event subscription
* Cleanup
---

### signalRService.js
Creates connection:
HubConnectionBuilder
   → withUrl(HUB_URL)
   → withAutomaticReconnect()
   → build()
---

## 🧠 Key Concepts
### 1. Event-driven UI
UI does NOT poll backend — it reacts to events.
---

### 2. Single Source of Truth
Messages stored in:
useState (inside ChatPanel)
---

### 3. Connection Lifecycle
Mount → Connect → Listen → Update UI → Cleanup on Unmount
---

## ⚠️ Common Bugs (VERY IMPORTANT)
### 🔴 Duplicate Messages
**Cause:**
* `connection.on()` registered multiple times
**Fix:**

useEffect(() => {
  connection.on("ReceiveMessage", handler);

  return () => {
    connection.off("ReceiveMessage");
  };
}, []);
---

## 🔐 Environment Config
REACT_APP_HUB_URL=https://localhost:7001/chatHub
---

## 🚀 Improvements
* Add global state (Redux/Zustand)
* Add typing indicator
* Add read receipts
* Add authentication (JWT)
* Add chat list sidebar
---

## 🧪 Testing Strategy
* Mock SignalR
* Simulate events
* Test UI rendering
---

## 🧠 Summary
Frontend responsibilities:
* Send message
* Listen for message
* Update UI

👉 It should NOT contain business logic
👉 It should NOT know database details
---
