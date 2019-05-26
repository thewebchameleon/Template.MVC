CREATE PROCEDURE [dbo].[sp_email_templates_get]
AS
BEGIN
	SELECT
		[ET].[Id],
		[ET].[Key],
		[ET].[Description],
		[ET].[Body],
		[ET].[Created_By],
		[ET].[Created_Date],
		[ET].[Updated_By],
		[ET].[Updated_Date],
		[ET].Is_Deleted
	FROM [Email_Template] [ET](NOLOCK)
	WHERE [ET].Is_Deleted = 0
END