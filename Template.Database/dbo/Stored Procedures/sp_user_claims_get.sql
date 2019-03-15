CREATE PROCEDURE [dbo].[sp_user_claims_get]
AS
BEGIN
	SELECT
		[UC].[Id],
		[UC].[Claim_Id],
		[UC].[User_Id],
		[UC].[Created_By],
		[UC].[Created_Date],
		[UC].[Updated_By],
		[UC].[Updated_Date],
		[UC].Is_Deleted
	FROM [User_Claim] [UC](NOLOCK)
	WHERE [UC].Is_Deleted = 0
END