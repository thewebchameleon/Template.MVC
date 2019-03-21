CREATE PROCEDURE [dbo].[sp_role_enable]
	@Role_d        INT,
	@Updated_By INT
AS
BEGIN
   UPDATE [Role]
   SET
		[Is_Enabled] = 1,
		[Updated_By] = @Updated_By,
		[Updated_Date] = GETDATE()
   WHERE
		[Id] = @Role_d
   SELECT
		@Role_d
END