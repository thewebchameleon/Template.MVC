CREATE PROCEDURE [dbo].[sp_sessions_get_by_date]
	@Date DATE
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
   WHERE  CONVERT(DATE, [S].[Created_Date]) = @Date
   AND [S].[Is_Deleted] = 0
   ORDER BY [S].[Created_Date] DESC
END