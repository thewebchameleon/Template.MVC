CREATE PROCEDURE [dbo].[sp_claims_get]
AS
BEGIN
   SELECT
		[C].[Id],
		[C].[Type],
		[C].[Value],
		[C].[Created_By],
		[C].[Created_Date],
		[C].[Updated_By],
		[C].[Updated_Date],
		[C].Is_Deleted
   FROM   [Claim] [C](NOLOCK)
   WHERE [C].Is_Deleted = 0
END