CREATE PROCEDURE [dbo].[sp_role_claim_create]
	@Role_Id     INT,
	@Claim_Id	INT,
	@Created_By  INT
AS
BEGIN
   INSERT INTO [dbo].[Role_Claim]
	    (
		[Claim_Id],
		[Role_Id],
		[Created_By],
		[Created_Date],
		[Updated_By],
		[Updated_Date],
		Is_Deleted
	    )
   VALUES
	    (
		@Claim_Id,
		@Role_Id,
		@Created_By,
		GETDATE(),
		@Created_By,
		GETDATE(),
		0
	    )
   SELECT
		SCOPE_IDENTITY() AS [Id]
END