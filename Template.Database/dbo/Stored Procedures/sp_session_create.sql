﻿CREATE PROCEDURE [dbo].[sp_session_create]
	@Guid      VARCHAR(256),
	@Created_By INT
AS
BEGIN
   INSERT INTO [Session]
	    (
		[Guid],
		[Created_By],
		[Created_Date],
		[Updated_By],
		[Updated_Date],
		Is_Deleted
	    )
   VALUES
	    (
		@Guid,
		@Created_By,
		GETDATE(),
		@Created_By,
		GETDATE(),
		0
	    )
   SELECT
		SCOPE_IDENTITY() AS [Id]
END