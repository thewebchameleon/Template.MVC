CREATE PROCEDURE [dbo].[sp_session_get_by_user_id]
	@User_Id INT
AS
BEGIN
   SELECT
		[S].[Id],
		[S].[User_Id],
		[S].[Created_By],
		[S].[Created_Date],
		[S].[Updated_By],
		[S].[Updated_Date],
		[S].Is_Deleted
   FROM   [Session] [S](NOLOCK)
   WHERE  [S].[User_Id] = @User_Id
   AND S.Is_Deleted = 0
END