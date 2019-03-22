CREATE PROCEDURE [dbo].[sp_session_log_event_create]
	@Session_Log_Id			INT,
	@Event_Id				INT,
	@Message				VARCHAR(MAX),
	@Created_By				INT
AS
BEGIN
   INSERT INTO [Session_Log_Event]
	    (
		[Session_Log_Id],
		[Event_Id],
		[Message],
        [Created_By],
        [Created_Date],
        [Updated_By],
        [Updated_Date],
        Is_Deleted
	    )
   VALUES
	    (
		@Session_Log_Id,
		@Event_Id,
		@Message,
		@Created_By,
		GETDATE(),
		@Created_By,
		GETDATE(),
		0
	    )
   SELECT
		SCOPE_IDENTITY() AS [Id]
END