CREATE PROCEDURE [dbo].[sp_user_update]
	@Id							INT,
	@Username					VARCHAR(256),
	@Email_Address				VARCHAR(256),
	@Registration_Confirmed     BIT,
	@First_Name					VARCHAR(256),
	@Last_Name					VARCHAR(256),
	@Mobile_Number				VARCHAR(30),
	@Password_Hash				VARCHAR(MAX),
	@Is_Locked_Out				BIT,
	@Lockout_End				DATETIME NULL,
	@Updated_By					INT,
	@Is_Enabled					BIT
AS
BEGIN
   UPDATE [User]
   SET
		[Username] = @Username,
		[Email_Address] = @Email_Address,
		[Registration_Confirmed] = @Registration_Confirmed,
		[First_Name] = @First_Name,
		[Last_Name] = @Last_Name,
		[Mobile_Number] = @Mobile_Number,
		[Password_Hash] = @Password_Hash,
		[Is_Locked_Out] = @Is_Locked_Out,
		[Lockout_End] = @Lockout_End,
		[Is_Enabled] = @Is_Enabled,
		[Updated_By] = @Updated_By,
		[Updated_Date] = GETDATE()
   WHERE
		[Id] = @Id
   SELECT
		@Id
END