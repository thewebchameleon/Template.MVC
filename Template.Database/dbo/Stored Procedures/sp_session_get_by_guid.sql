CREATE PROCEDURE [dbo].[sp_session_get_by_guid]
	@Guid VARCHAR(60)
AS
BEGIN
   SELECT
		[S].[Id],
		[S].[Guid],
		[S].[User_Id],
		[S].[Created_By],
		[S].[Created_Date],
		[S].[Updated_By],
		[S].[Updated_Date],
		[S].Is_Deleted
   FROM   [Session] [S](NOLOCK)
   WHERE  [S].[Guid] = @Guid
   AND S.Is_Deleted = 0
END