CREATE PROCEDURE [dbo].[sp_user_get_by_id]
	@UserId INT
AS
BEGIN
   SELECT
		[U].[Id],
		[U].[Username],
		[U].[Email],
		[U].[Registration_Confirmed],
		[U].[Password_Hash],
		[U].[Is_Locked_Out],
		[U].[Lockout_End],
		[U].[Created_By],
		[U].[Created_Date],
		[U].[Updated_By],
		[U].[Updated_Date],
		[U].Is_Deleted
   FROM   [User] [U](NOLOCK)
   WHERE  [U].[Id] = @UserId
   AND U.Is_Deleted = 0
END