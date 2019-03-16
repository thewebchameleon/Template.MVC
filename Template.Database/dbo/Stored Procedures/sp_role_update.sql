CREATE PROCEDURE [dbo].[sp_role_update]
	@Id             INT,
	@Name           VARCHAR(256),
	@Description    VARCHAR(256),
	@Is_Enabled		BIT,
	@Updated_By      INT
AS
BEGIN
   UPDATE [Role]
   SET
		[Name] = @Name,
		[Description] = @Description,
		[Is_Enabled] = @Is_Enabled,
		[Updated_By] = @Updated_By,
		[Updated_Date] = GETDATE()
   WHERE
		[Id] = @Id
   SELECT
		@Id
END