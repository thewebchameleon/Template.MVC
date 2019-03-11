CREATE PROCEDURE [dbo].[sp_user_update]
	@Id                 INT,
	@Username           VARCHAR(256),
	@UsernameNormalized VARCHAR(256),
	@Email              VARCHAR(256),
	@EmailNormalized    VARCHAR(256),
	@EmailConfirmed     BIT,
	@FirstName          VARCHAR(256),
	@LastName           VARCHAR(256),
	@GenderId           INT,
	@Latitude           DECIMAL(9, 6),
	@Longitude          DECIMAL(9, 6),
	@Suburb             VARCHAR(200),
	@MobileNumber       VARCHAR(30),
	@PasswordHash       VARCHAR(MAX),
	@Configuration      VARCHAR(500),
	@IsLockedOut        BIT,
	@LockoutEnd         DATETIME NULL,
	@UpdatedBy          INT
AS
BEGIN
   UPDATE [User]
   SET
		[Username] = @Username,
		[Email] = @Email,
		[First_Name] = @FirstName,
		[Last_Name] = @LastName,
		[Mobile_Number] = @MobileNumber,
		[Password_Hash] = @PasswordHash,
		[Is_Locked_Out] = @IsLockedOut,
		[Lockout_End] = @LockoutEnd,
		[Updated_By] = @UpdatedBy,
		[Updated_Date] = GETDATE()
   WHERE
		[Id] = @Id
   SELECT
		@Id
END