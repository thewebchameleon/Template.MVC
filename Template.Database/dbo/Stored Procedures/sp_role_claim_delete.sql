CREATE PROCEDURE [dbo].[sp_role_claim_delete]
	@Role_Id     INT,
	@Claim_Id	VARCHAR(256),
	@Updated_By  INT
AS
BEGIN
   DECLARE
		@Id INT;
   UPDATE [Role_Claim]
   SET
		@Id = [Id],
		Is_Deleted = 1,
		[Updated_By] = @Updated_By,
		[Updated_Date] = GETDATE()
   WHERE
		[Role_Id] = @Role_Id
		AND [Claim_Id] = @Claim_Id
   SELECT
		@Id AS [Id]
END