CREATE PROCEDURE [dbo].[sp_user_update]
	@Id							INT,
	@Username					VARCHAR(256),
	@Email_Address				VARCHAR(256),
	@Registration_Confirmed     BIT,
	@First_Name					VARCHAR(256),
	@Last_Name					VARCHAR(256),
	@Mobile_Number				VARCHAR(30),
	@Password_Hash				VARCHAR(MAX),
	@Updated_By					INT
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
		[Password_Hash] = ISNULL(@Password_Hash, [Password_Hash]),
		[Updated_By] = @Updated_By,
		[Updated_Date] = GETDATE()
   WHERE
		[Id] = @Id
   SELECT
		@Id
END