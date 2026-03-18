-- ============================================================
-- Script: 03_SqlAgentCleanupJob.sql
-- Description: Optional SQL Server Agent Job as a safety net
-- for message cleanup. Runs every hour independently of the app.
--
-- WHY BOTH IHostedService AND SQL JOB?
--   IHostedService = primary cleanup (runs inside the app)
--   SQL Agent Job  = safety net (runs even when app is down)
--
-- Run this script ONLY if SQL Server Agent is available.
-- ============================================================

USE msdb;
GO

-- Drop existing job if it exists (idempotent script)
IF EXISTS (SELECT job_id FROM msdb.dbo.sysjobs WHERE name = N'ChattingApp - Delete Old Messages')
BEGIN
    EXEC msdb.dbo.sp_delete_job
        @job_name = N'ChattingApp - Delete Old Messages',
        @delete_unused_schedule = 1;
    PRINT 'Old job dropped.';
END
GO

-- ============================================================
-- Create the Job
-- ============================================================
EXEC msdb.dbo.sp_add_job
    @job_name       = N'ChattingApp - Delete Old Messages',
    @enabled        = 1,
    @description    = N'Deletes chat messages older than 12 hours. Runs every hour as a safety net.',
    @category_name  = N'[Uncategorized (Local)]',
    @owner_login_name = N'sa';
GO

-- ============================================================
-- Add a Job Step: calls our stored procedure
-- ============================================================
EXEC msdb.dbo.sp_add_jobstep
    @job_name        = N'ChattingApp - Delete Old Messages',
    @step_name       = N'Execute usp_DeleteOldMessages',
    @subsystem       = N'TSQL',
    @database_name   = N'ChattingAppDb',
    @command         = N'EXEC dbo.usp_DeleteOldMessages @OlderThanHours = 12;',
    @on_success_action = 1,  -- 1 = Quit with success
    @on_fail_action    = 2,  -- 2 = Quit with failure
    @retry_attempts    = 3,
    @retry_interval    = 5;  -- 5 minutes between retries
GO

-- ============================================================
-- Schedule: every 1 hour, starting immediately
-- ============================================================
EXEC msdb.dbo.sp_add_schedule
    @schedule_name       = N'Every 1 Hour',
    @freq_type           = 4,       -- Daily
    @freq_interval       = 1,       -- Every 1 day
    @freq_subday_type    = 8,       -- 8 = Hours
    @freq_subday_interval = 1,      -- Every 1 hour
    @active_start_time   = 000000;  -- Midnight
GO

EXEC msdb.dbo.sp_attach_schedule
    @job_name      = N'ChattingApp - Delete Old Messages',
    @schedule_name = N'Every 1 Hour';
GO

-- ============================================================
-- Assign job to the local server
-- ============================================================
EXEC msdb.dbo.sp_add_jobserver
    @job_name   = N'ChattingApp - Delete Old Messages',
    @server_name = N'(LOCAL)';
GO

PRINT 'SQL Agent Job [ChattingApp - Delete Old Messages] created successfully.';
GO
