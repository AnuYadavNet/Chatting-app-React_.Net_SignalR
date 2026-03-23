# 🎨 Frontend Documentation

## Tech Stack
- React
- SignalR Client
---

## Structure
src/
- components/
  - ChatPanel.jsx
  - MessageBubble.jsx
  - ConnectionBadge.jsx
- hooks/
  - useChatSignalR.js
- services/
  - signalRService.js
- App.jsx
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

## Core Flow
1. User types message
2. ChatPanel calls sendMessage()
3. Hook invokes SignalR method
4. Backend processes message
5. Message received via connection.on()
6. UI updates instantly
---

## Key Components
### ChatPanel
- Handles UI and input
- Sends messages

### MessageBubble
- Displays messages

### ConnectionBadge
- Shows connection status
---

## Hooks
### useChatSignalR
- Manages connection lifecycle
- Handles send/receive
---

## Services
### signalRService
- Builds SignalR connection
---

## Frontend Responsibilities
- UI rendering
- Real-time updates
- Connection handling
---

## Improvements (AI Agent Tasks)
- Add global state (Redux/Zustand)
- Add message pagination
- Add typing indicators
- Add reconnection UI handling
- Add error boundaries