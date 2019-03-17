CREATE PROCEDURE [dbo].[sp_sessions_get_by_start_date]
	@Start_Date DATETIME
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
   WHERE  [S].[Created_Date] >= @Start_Date
   AND [S].[Is_Deleted] = 0
   ORDER BY [S].[Created_Date] DESC
END