﻿CREATE PROCEDURE [dbo].[sp_user_get_by_username]
	@Username VARCHAR(256)
AS
BEGIN
   SELECT
		[U].[Id],
		[U].[Username],
		[U].[Email_Address],
		[U].[Registration_Confirmed],
		[U].[Password_Hash],
		[U].[Is_Locked_Out],
		[U].[Lockout_End],
		[U].[Created_By],
		[U].[Created_Date],
		[U].[Updated_By],
		[U].[Updated_Date],
		[U].Is_Deleted,
		[U].Is_Enabled
   FROM   [User] [U](NOLOCK)
   WHERE  [U].[Username] = @Username;
END