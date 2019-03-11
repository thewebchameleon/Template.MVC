CREATE PROCEDURE [dbo].[sp_session_add_user_by_guid]
	@Guid      VARCHAR(256) NULL,
	@User_Id    INT NULL,
	@Updated_By INT NULL
AS
BEGIN
   DECLARE
		@SessionId INT;
   UPDATE [Session]
   SET
		@SessionId = [Id],
		[User_Id] = @User_Id,
		[Updated_By] = @Updated_By,
		[Updated_Date] = GETDATE()
   WHERE
		[Guid] = @Guid
   SELECT
		@SessionId AS [Id]
END