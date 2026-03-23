# 🗄️ Database Documentation

## 📂 Database Structure
Database/
└── Scripts/
    ├── 01_CreateTables.sql
    ├── 02_StoredProcedures.sql
    ├── 03_SqlAgentCleanupJob.sql
    └── 04_CreateUsersTable.sql

## Database
- SQL Server
- Database Name: ChattingAppDb
---

## Tables
### Messages
- MessageId (PK)
- Sender
- Receiver
- Content
- Timestamp
---

## Stored Procedures
### usp_InsertMessage
- Inserts new message
- Returns MessageId

### (Other Procedures)
- Fetch messages
- Cleanup messages
---

## Scripts
1. 01_CreateTables.sql
2. 02_StoredProcedures.sql
3. 03_SqlAgentCleanupJob.sql (optional)
---

## Cleanup Strategy
### Primary
- IHostedService (app-level cleanup)

### Secondary
- SQL Agent Job (fallback)
---

## Database Responsibilities
- Persist messages
- Execute stored procedures
- Maintain performance via indexes
---

## Improvements (AI Agent Tasks)
- Add indexes on Sender/Receiver
- Add soft delete
- Add message status (Read/Delivered)
- Add partitioning for large data
- Add audit logging