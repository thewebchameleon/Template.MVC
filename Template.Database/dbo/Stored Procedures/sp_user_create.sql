CREATE PROCEDURE [dbo].[sp_user_create]
	@Username					VARCHAR(256),
	@Email_Address				VARCHAR(256),
	@Registration_Confirmed     BIT,
	@First_Name					VARCHAR(256),
	@Last_Name					VARCHAR(256),
	@Mobile_Number				VARCHAR(30),
	@Password_Hash				VARCHAR(MAX),
	@Is_Locked_Out				BIT,
	@Lockout_End				DATETIME NULL,
	@Created_By					INT
AS
BEGIN
   INSERT INTO [User]
	    (
		[Username],
		[Email_Address],
		[Registration_Confirmed],
		[First_Name],
		[Last_Name],
		[Mobile_Number],
		[Password_Hash],
		[Is_Locked_Out],
		[Lockout_End],
		[Created_By],
		[Created_Date],
		[Updated_By],
		[Updated_Date],
		Is_Deleted
	    )
   VALUES
	    (
		@Username,
		@Email_Address,
		@Registration_Confirmed,
		@First_Name,
		@Last_Name,
		@Mobile_Number,
		@Password_Hash,
		@Is_Locked_Out,
		@Lockout_End,
		@Created_By,
		GETDATE(),
		@Created_By,
		GETDATE(),
		0
	    )
   SELECT
		SCOPE_IDENTITY() AS [Id]
END