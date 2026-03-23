# ⚙️ Backend Documentation

## 📂 Backend Folder Structure
Backend/
└── ChattingApp/
    ├── ChattingApp.API/
    │   ├── Controllers/
    │   │   ├── AuthController.cs
    │   │   └── ChatController.cs
    │   ├── Hubs/
    │   │   └── ChatHub.cs
    │   ├── Program.cs
    │   └── appsettings.json
    │
    ├── ChattingApp.Application/
    │   ├── BackgroundServices/
    │   ├── DTOs/
    │   │   ├── Auth/
    │   │   └── MessageDto.cs
    │   ├── Interfaces/
    │   ├── Services/
    │
    ├── ChattingApp.Domain/
    │   └── Entities/
    │       ├── Message.cs
    │       └── User.cs
    │
    └── ChattingApp.Infrastructure/
        ├── Data/
        └── Repositories/

## Tech Stack
- .NET 8
- ASP.NET Core Web API
- SignalR
- Dapper
- SQL Server
- JWT Authentication
---

## Responsibilities
- Authentication & Authorization
- Real-time messaging
- Business logic execution
- Data persistence
- Background processing
---

## Core Modules

### 1. Authentication Module
**Components**
- AuthController
- AuthService
- IUserRepository

**Flow**
1. User registers/logs in
2. Credentials validated
3. JWT token generated
4. Token returned to client
---

### 2. Chat Module
**Components**
- ChatHub (real-time)
- ChatController (optional REST)
- ChatService
- ChatRepository

**Flow**
1. Client invokes SendMessage
2. Hub receives message
3. Service validates & processes
4. Repository saves message
5. Message broadcasted to group
---

### 3. Repository Layer
- ChatRepository → message operations
- UserRepository → user operations

Uses:
- Dapper
- Stored Procedures
---

### 4. Background Services
#### MessageCleanupService
- Deletes old messages periodically
- Prevents DB bloat
---

## Important Patterns
### Dependency Injection
- All services registered via DI
- Interfaces used everywhere

### Async/Await
- All DB & IO operations async
---

## Configuration
### appsettings.json
- DB connection string
- JWT secret (should move to secure store)
---

## Security
- JWT authentication
- Protected endpoints
- Token validation middleware
---

## Error Handling (Recommended)
- Global exception middleware (add if missing)
- Standard API response format
---

## Logging (Recommended)
- Add Serilog or built-in logging
- Log:
  - Requests
  - Errors
  - SignalR events
---

## Improvements (AI Tasks)
- Add refresh tokens
- Add role-based authorization
- Add MediatR (CQRS pattern)
- Add unit + integration tests
- Add rate limiting (critical for chat)
- Add retry policies for DB