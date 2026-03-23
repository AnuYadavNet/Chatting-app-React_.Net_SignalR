# 🏗️ System Architecture

## Overview
Real-time chat application built using:
- .NET 8 (Backend)
- SignalR (Real-time communication)
- React (Frontend)
- SQL Server (Database)
- JWT Authentication
---

## Architecture Style
- Clean Architecture
- Layered design
- Dependency inversion (outer → inner)
---

## 📂 High-Level Project Structure
Root/
│
├── Backend/
│   └── ChattingApp/
│       ├── ChattingApp.API/
│       ├── ChattingApp.Application/
│       ├── ChattingApp.Domain/
│       └── ChattingApp.Infrastructure/
│
├── Frontend/
│   └── chatting-app/
│
└── Database/
    └── Scripts/

## High-Level Layers

### 1. Presentation Layer (API)
Handles incoming requests and real-time communication.

**Components**
- AuthController (JWT authentication)
- ChatController (REST APIs)
- ChatHub (SignalR real-time messaging)
---

### 2. Application Layer
Contains business logic and use cases.

**Components**
- Services:
  - AuthService
  - ChatService
- Interfaces:
  - IAuthService
  - IChatService
  - IChatRepository
  - IUserRepository
- DTOs:
  - Auth (LoginDto, RegisterDto, AuthResponseDto)
  - MessageDto
- Background:
  - MessageCleanupService
---

### 3. Domain Layer
Core business models.

**Entities**
- User
- Message
---

### 4. Infrastructure Layer
Handles external systems (DB, persistence).

**Components**
- DbConnectionFactory
- ChatRepository
- UserRepository
---

## Frontend Architecture
- React with routing + protected routes
- JWT-based authentication
- SignalR integration

**Main Flow**
- Login/Register → JWT stored
- Authenticated routes enabled
- SignalR connects with auth context
---

## Real-Time Communication Flow
Frontend → SignalR → ChatHub → ChatService → Repository → DB  
→ Broadcast via SignalR → Clients update UI

---

## Security Architecture
- JWT-based authentication
- Protected API routes
- Token-based frontend communication
---

## Key Design Decisions
### Clean Architecture
- Decoupled layers
- Testable services
- Replaceable infrastructure

### Repository Pattern
- No SQL leakage outside infrastructure
- Centralized DB logic

### SignalR Groups
- Stable conversation identity
- Handles reconnections gracefully

### Dapper
- High-performance DB operations
- Stored procedure support
---

## Background Processing
### MessageCleanupService
- Periodic cleanup of old messages
- Runs inside application (IHostedService)

### SQL Agent Job (Optional)
- Backup cleanup mechanism
---

## Scalability Considerations
- Stateless API (JWT-based)
- Can scale horizontally
- Future: Azure SignalR Service
---

## Known Gaps / Improvements
- Rate limiting (SendMessage)
- Observability (logging + tracing)
- Distributed caching (Redis)
- Health checks (/healthz)