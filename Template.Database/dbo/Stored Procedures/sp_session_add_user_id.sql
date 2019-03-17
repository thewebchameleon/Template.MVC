CREATE PROCEDURE [dbo].[sp_session_add_user_id]
	@Id			INT NULL,
	@User_Id    INT NULL,
	@Updated_By INT NULL
AS
BEGIN
   UPDATE [Session]
   SET
		[User_Id] = @User_Id,
		[Updated_By] = @Updated_By,
		[Updated_Date] = GETDATE()
   WHERE
		[Id] = @Id

   SELECT @Id AS [Id]
END