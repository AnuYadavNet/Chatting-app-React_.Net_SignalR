-- ============================================================
-- Script: 04_CreateUsersTable.sql
-- Description: Creates the Users table and associated SPs
-- ============================================================

USE ChattingAppDb;
GO

-- Table: Users
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Users
    (
        UserId          INT IDENTITY(1,1)   NOT NULL,
        Username        NVARCHAR(100)       NOT NULL UNIQUE,
        PasswordHash    NVARCHAR(500)       NOT NULL,
        Role            NVARCHAR(50)        NOT NULL CONSTRAINT DF_Users_Role DEFAULT ('User'),
        CreatedAt       DATETIME2(7)        NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (GETUTCDATE()),

        CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId ASC)
    );
    PRINT 'Table dbo.Users created.';
END
ELSE
BEGIN
    PRINT 'Table dbo.Users already exists.';
END
GO

-- Stored Procedure: usp_CreateUser
CREATE OR ALTER PROCEDURE dbo.usp_CreateUser
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(500),
    @Role NVARCHAR(50),
    @NewUserId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Users (Username, PasswordHash, Role)
    VALUES (@Username, @PasswordHash, @Role);

    SET @NewUserId = SCOPE_IDENTITY();
END
GO

-- Stored Procedure: usp_GetUserByUsername
CREATE OR ALTER PROCEDURE dbo.usp_GetUserByUsername
    @Username NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UserId, Username, PasswordHash, Role, CreatedAt
    FROM dbo.Users
    WHERE Username = @Username;
END
GO
