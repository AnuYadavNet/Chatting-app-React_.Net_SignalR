-- ============================================================
-- Script: 02_StoredProcedures.sql
-- Description: All stored procedures for the ChattingApp
-- Run AFTER 01_CreateTables.sql
-- ============================================================

USE ChattingAppDb;
GO

-- ============================================================
-- SP 1: usp_InsertMessage
-- Inserts a new chat message and returns the new MessageId
-- ============================================================
IF OBJECT_ID('dbo.usp_InsertMessage', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_InsertMessage;
GO

CREATE PROCEDURE dbo.usp_InsertMessage
    @SenderId       NVARCHAR(100),
    @ReceiverId     NVARCHAR(100),
    @MessageText    NVARCHAR(2000),
    @Timestamp      DATETIME2(7),
    @NewMessageId   INT OUTPUT          -- Returns the generated MessageId
AS
BEGIN
    SET NOCOUNT ON;

    -- Guard: prevent empty messages from reaching the DB
    IF LEN(LTRIM(RTRIM(@MessageText))) = 0
    BEGIN
        RAISERROR('MessageText cannot be empty.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Messages
        (SenderId, ReceiverId, MessageText, Timestamp)
    VALUES
        (@SenderId, @ReceiverId, LTRIM(RTRIM(@MessageText)), @Timestamp);

    -- Return the auto-generated PK to the application
    SET @NewMessageId = SCOPE_IDENTITY();
END
GO

PRINT 'usp_InsertMessage created.';

-- ============================================================
-- SP 2: usp_GetChatHistory
-- Fetches all messages exchanged between two users (bi-directional),
-- ordered by Timestamp ASC (oldest first → natural chat flow)
-- ============================================================
IF OBJECT_ID('dbo.usp_GetChatHistory', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_GetChatHistory;
GO

CREATE PROCEDURE dbo.usp_GetChatHistory
    @UserA  NVARCHAR(100),
    @UserB  NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        MessageId,
        SenderId,
        ReceiverId,
        MessageText,
        Timestamp
    FROM
        dbo.Messages
    WHERE
        -- Conversation is bi-directional: A→B or B→A
        (SenderId = @UserA AND ReceiverId = @UserB)
        OR
        (SenderId = @UserB AND ReceiverId = @UserA)
    ORDER BY
        Timestamp ASC;
END
GO

PRINT 'usp_GetChatHistory created.';

-- ============================================================
-- SP 3: usp_DeleteOldMessages
-- Deletes all messages older than N hours (default: 12)
-- Called by the .NET IHostedService background worker every hour
-- ============================================================
IF OBJECT_ID('dbo.usp_DeleteOldMessages', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_DeleteOldMessages;
GO

CREATE PROCEDURE dbo.usp_DeleteOldMessages
    @OlderThanHours INT = 12    -- Default: 12 hours
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CutoffTime DATETIME2(7) = DATEADD(HOUR, -@OlderThanHours, GETUTCDATE());
    DECLARE @DeletedCount INT = 0;

    -- Delete in batches of 1000 to avoid excessive locking
    WHILE 1 = 1
    BEGIN
        DELETE TOP (1000)
        FROM dbo.Messages
        WHERE Timestamp < @CutoffTime;

        SET @DeletedCount = @DeletedCount + @@ROWCOUNT;

        -- Stop when no more rows to delete
        IF @@ROWCOUNT = 0
            BREAK;
    END

    -- Return count for logging/monitoring purposes
    SELECT @DeletedCount AS DeletedMessageCount;

    PRINT CONCAT('Deleted ', @DeletedCount, ' messages older than ', @OlderThanHours, ' hours.');
END
GO

PRINT 'usp_DeleteOldMessages created.';
PRINT 'All stored procedures installed successfully.';
GO
