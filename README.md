# ⚡ Real time Chatting App
### Real-time Chat · .NET 8 · SignalR · React · SQL Server

---

## 🏗️ Architecture Overview

```
┌──────────────────────────────────────────────────────────────────┐
│                        CLEAN ARCHITECTURE                         │
│                                                                    │
│  ┌──────────────────────────────────────────────────────────────┐ │
│  │  PRESENTATION LAYER (ChattingApp.API)                        │ │
│  │  ├── Controllers/ChatController.cs  (REST endpoints)         │ │
│  │  └── Hubs/ChatHub.cs               (SignalR real-time)       │ │
│  └────────────────────────┬─────────────────────────────────────┘ │
│                           │  depends on ↓                          │
│  ┌──────────────────────────────────────────────────────────────┐ │
│  │  APPLICATION LAYER (ChattingApp.Application)                 │ │
│  │  ├── Interfaces/IChatService.cs                              │ │
│  │  ├── Interfaces/IChatRepository.cs                           │ │
│  │  ├── Services/ChatService.cs       (business logic)          │ │
│  │  ├── DTOs/MessageDto.cs                                      │ │
│  │  └── BackgroundServices/MessageCleanupService.cs             │ │
│  └────────────────────────┬─────────────────────────────────────┘ │
│                           │  depends on ↓                          │
│  ┌──────────────────────────────────────────────────────────────┐ │
│  │  DOMAIN LAYER (ChattingApp.Domain)                           │ │
│  │  └── Entities/Message.cs           (pure domain entity)      │ │
│  └──────────────────────────────────────────────────────────────┘ │
│  ┌──────────────────────────────────────────────────────────────┐ │
│  │  INFRASTRUCTURE LAYER (ChattingApp.Infrastructure)           │ │
│  │  ├── Data/DbConnectionFactory.cs                             │ │
│  │  └── Repositories/ChatRepository.cs (Dapper + SQL Server)    │ │
│  └──────────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────────┘

React Frontend
┌──────────────────────────────────────────────────────────────────┐
│  App.jsx  (split-screen layout)                                   │
│  ├── ChatPanel.jsx (User A)  ←→  ChatPanel.jsx (User B)          │
│  │   ├── MessageBubble.jsx                                        │
│  │   └── ConnectionBadge.jsx                                      │
│  ├── hooks/useChatSignalR.js  (SignalR connection lifecycle)      │
│  └── services/signalRService.js  (HubConnectionBuilder factory)   │
└──────────────────────────────────────────────────────────────────┘
```

---

## 📂 Project Structure

```
P:\Chatting app17Mar2026\
│
├── Backend\
│   └── ChattingApp\
│       ├── ChattingApp.API\
│       │   ├── Controllers\
│       │   │   └── ChatController.cs
│       │   ├── Hubs\
│       │   │   └── ChatHub.cs
│       │   ├── Program.cs
│       │   ├── appsettings.json
│       │   └── ChattingApp.API.csproj
│       │
│       ├── ChattingApp.Application\
│       │   ├── BackgroundServices\
│       │   │   └── MessageCleanupService.cs
│       │   ├── DTOs\
│       │   │   └── MessageDto.cs
│       │   ├── Interfaces\
│       │   │   ├── IChatRepository.cs
│       │   │   └── IChatService.cs
│       │   ├── Services\
│       │   │   └── ChatService.cs
│       │   └── ChattingApp.Application.csproj
│       │
│       ├── ChattingApp.Domain\
│       │   ├── Entities\
│       │   │   └── Message.cs
│       │   └── ChattingApp.Domain.csproj
│       │
│       └── ChattingApp.Infrastructure\
│           ├── Data\
│           │   └── DbConnectionFactory.cs
│           ├── Repositories\
│           │   └── ChatRepository.cs
│           └── ChattingApp.Infrastructure.csproj
│
├── Database\
│   └── Scripts\
│       ├── 01_CreateTables.sql
│       ├── 02_StoredProcedures.sql
│       └── 03_SqlAgentCleanupJob.sql   ← Optional safety net
│
└── Frontend\
    └── chatting-app\
        ├── public\
        │   └── index.html
        ├── src\
        │   ├── components\
        │   │   ├── ChatPanel.jsx
        │   │   ├── MessageBubble.jsx
        │   │   └── ConnectionBadge.jsx
        │   ├── hooks\
        │   │   └── useChatSignalR.js
        │   ├── services\
        │   │   └── signalRService.js
        │   ├── App.jsx
        │   ├── App.css
        │   └── index.js
        ├── .env.example
        └── package.json
```

---

## ⚙️ Setup Instructions

### Step 1 — Database Setup

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your SQL Server instance
3. Run scripts in order:
   ```
   01_CreateTables.sql       → Creates ChattingAppDb + Messages table + indexes
   02_StoredProcedures.sql   → Installs all 3 stored procedures
   03_SqlAgentCleanupJob.sql → (Optional) SQL Agent job as cleanup safety net
   ```

### Step 2 — Backend Setup

1. Open `Backend\ChattingApp\` in **Visual Studio 2022** or **VS Code**

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Update the connection string in `ChattingApp.API\appsettings.json`:
   ```json
   "ConnectionStrings": {
     "ChattingAppDb": "Server=YOUR_SERVER;Database=ChattingAppDb;
                       User Id=YOUR_USER;Password=YOUR_PASSWORD;
                       TrustServerCertificate=True;"
   }
   ```

4. Set `ChattingApp.API` as the startup project and run:
   ```bash
   cd Backend/ChattingApp/ChattingApp.API
   dotnet run
   ```
   The API will start at `https://localhost:7001` (check launchSettings.json for exact port)

5. Verify Swagger UI is working at:
   ```
   https://localhost:7001/swagger
   ```

### Step 3 — Frontend Setup

1. Navigate to the frontend folder:
   ```bash
   cd Frontend/chatting-app
   ```

2. Copy the env example and configure:
   ```bash
   copy .env.example .env
   ```
   Update `.env` if your backend port differs from `7001`.

3. Install dependencies:
   ```bash
   npm install
   ```

4. Start the React development server:
   ```bash
   npm start
   ```
   The app will open at `http://localhost:3000`

---

## 🔄 Real-Time Flow

```
User A types message and presses Enter / Send
         │
         ▼
ChatPanel.jsx calls sendMessage(text)
         │
         ▼
useChatSignalR hook: connection.invoke("SendMessage", dto)
         │
         ▼  [WebSocket frame]
ChatHub.SendMessage(dto)
         │
         ├─► ChatService.SendMessageAsync(dto)
         │       │
         │       ├─► Validates input
         │       ├─► Maps DTO → Message entity
         │       └─► ChatRepository.InsertMessageAsync(message)
         │               │
         │               └─► SQL: EXEC usp_InsertMessage → returns MessageId
         │
         └─► Clients.Group("chat_alice_bob").SendAsync("ReceiveMessage", savedMessageDto)
                  │
                  ├──► User A panel: connection.on("ReceiveMessage") → setMessages(...)
                  └──► User B panel: connection.on("ReceiveMessage") → setMessages(...)

Both UIs update instantly — zero page refresh needed.
```

---

## 🧠 Architecture & Design Decisions

### Why Clean Architecture?
- **Testability**: Each layer can be unit-tested in isolation
- **Maintainability**: Swap SQL Server for PostgreSQL by only touching Infrastructure
- **SOLID compliance**: Interfaces everywhere; concrete classes registered in DI

### Why Repository Pattern?
- Encapsulates all data access in one place
- Controllers and Services never write SQL — they call repository methods
- Makes switching from Dapper to EF Core trivial

### Why IHostedService instead of SQL Agent Job?
| Factor | IHostedService | SQL Job |
|--------|---------------|---------|
| Deployment | Ships with app — no DBA needed | Requires SQL Server Agent |
| Cloud-friendly | ✅ Works on Azure/Docker | ❌ Not available on Azure SQL Basic |
| Testable | ✅ Full .NET testing | ❌ Requires SQL environment |
| Safety net | ❌ App must be running | ✅ Runs independently |

**Decision**: Use IHostedService as primary, SQL Job as optional safety net.

### Why SignalR Groups over direct Connection IDs?
- Connection IDs change on every reconnect
- Groups persist the concept of a "conversation" across reconnections
- Group name `chat_alice_bob` (sorted) is deterministic from both users' sides

### Why Dapper over Entity Framework?
- Stored procedures are first-class citizens; Dapper maps results directly
- No ORM overhead for high-frequency message inserts
- Full control over SQL execution plan via stored procedures

---

## 🚀 Production Checklist

- [ ] Replace hardcoded UserA/UserB with JWT authentication
- [ ] Add HTTPS certificate in production (not dev cert)
- [ ] Set `REACT_APP_HUB_URL` to production domain
- [ ] Enable SignalR Azure Service for horizontal scaling
- [ ] Add rate limiting on the SendMessage hub method
- [ ] Switch connection string to Azure Key Vault / Secret Manager
- [ ] Enable SQL Server encrypted connection (`Encrypt=True`)
- [ ] Add health check endpoint (`/healthz`)

---

*Generated: 17 March 2026 | ChattingApp17Mar2026*
