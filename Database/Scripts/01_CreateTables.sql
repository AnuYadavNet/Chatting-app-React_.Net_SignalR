-- ============================================================
-- Script: 01_CreateTables.sql
-- Description: Creates the ChattingAppDb database and Messages table
-- Run this FIRST before any stored procedures or jobs
-- ============================================================

USE master;
GO

-- Create database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ChattingAppDb')
BEGIN
    CREATE DATABASE ChattingAppDb;
    PRINT 'Database ChattingAppDb created.';
END
GO

USE ChattingAppDb;
GO

-- ============================================================
-- Table: Messages
-- Stores all chat messages between any two users
-- ============================================================
IF NOT EXISTS (
    SELECT * FROM sys.tables WHERE name = 'Messages' AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
    CREATE TABLE dbo.Messages
    (
        MessageId   INT IDENTITY(1,1)   NOT NULL,   -- Auto-incrementing PK
        SenderId    NVARCHAR(100)        NOT NULL,   -- User who sent the message
        ReceiverId  NVARCHAR(100)        NOT NULL,   -- User who receives the message
        MessageText NVARCHAR(2000)       NOT NULL,   -- Message content (max 2000 chars)
        Timestamp   DATETIME2(7)        NOT NULL    -- UTC timestamp of message creation
            CONSTRAINT DF_Messages_Timestamp DEFAULT (GETUTCDATE()),

        CONSTRAINT PK_Messages PRIMARY KEY CLUSTERED (MessageId ASC)
    );

    -- Index to speed up chat history queries between two users
    CREATE NONCLUSTERED INDEX IX_Messages_SenderReceiver_Timestamp
        ON dbo.Messages (SenderId, ReceiverId, Timestamp ASC)
        INCLUDE (MessageText);

    -- Index to speed up the deletion job (filter by Timestamp)
    CREATE NONCLUSTERED INDEX IX_Messages_Timestamp
        ON dbo.Messages (Timestamp ASC);

    PRINT 'Table dbo.Messages created with indexes.';
END
ELSE
BEGIN
    PRINT 'Table dbo.Messages already exists. Skipping creation.';
END
GO
