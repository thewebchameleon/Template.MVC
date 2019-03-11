CREATE PROCEDURE [dbo].[sp_session_delete]
	@Id     INT,
	@Updated_By	INT
AS
BEGIN
   UPDATE [Session]
   SET
		Is_Deleted = 1,
		[Updated_By] = @Updated_By,
		[Updated_Date] = GETDATE()
   WHERE
		[Id] = @Id
   SELECT
		@Id AS [Id]
END