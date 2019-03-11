CREATE PROCEDURE [dbo].[sp_user_duplicate_check]
	@Email_Address		VARCHAR(256) NULL,
	@Mobile_Number		VARCHAR(30) NULL,
	@Username			VARCHAR(256) NULL,
	@User_Id			INT NULL
AS
BEGIN
   SELECT TOP 1
		[U].[Id]
   FROM   [User] [U](NOLOCK)
   WHERE  ([U].[Email] = @Email_Address
		 OR [U].[Mobile_Number] = @Mobile_Number
		 OR [U].[Username] = @Username)
		AND [U].[Id] != ISNULL(@User_Id, 0)
END