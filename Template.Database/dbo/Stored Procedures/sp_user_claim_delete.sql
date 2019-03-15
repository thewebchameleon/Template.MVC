CREATE PROCEDURE [dbo].[sp_user_claim_delete]
	@User_Id    INT,
	@Claim_Id    INT,
	@Updated_By INT
AS
BEGIN
   DECLARE
		@UserClaimId INT;
   UPDATE [User_Claim]
   SET
		@UserClaimId = [Id],
		Is_Deleted = 1,
		[Updated_By] = @Updated_By,
		[Updated_Date] = GETDATE()
   WHERE
		[User_Id] = @User_Id
		AND [Claim_Id] = @Claim_Id
   SELECT
		@UserClaimId AS [Id]
END