CREATE PROCEDURE [dbo].[sp_role_claim_create]
	@Role_Id     INT,
	@Claim_Id	INT,
	@Created_By  INT
AS
BEGIN
		-- find existing record else create a new one
	IF EXISTS
	(
		SELECT
				1
		FROM	[Role_Claim] (NOLOCK)
		WHERE	[Role_Id] = @Role_Id
		AND		[Claim_Id] = @Claim_Id
	)
	BEGIN

		-- reactivate deleted record
		DECLARE @Is_Deleted BIT = 0, 
				@Id INT;

		SELECT
			 @Is_Deleted = [Is_Deleted],
			 @Id = [Id]
		FROM   [Role_Claim] (NOLOCK)
		WHERE  [Role_Id] = @Role_Id
			 AND [Claim_Id] = @Claim_Id

		IF @Is_Deleted = 1
		BEGIN
			UPDATE [Role_Claim]
			SET 
				[Is_Deleted] = 0,
				Updated_By = Created_By,
				Updated_Date = GETDATE()
		END

		SELECT @Id AS [Id]
	END 
	ELSE
	BEGIN
		INSERT INTO [Role_Claim]
		(
				[Role_Id],
				[Claim_Id],
				[Created_By],
				[Created_Date],
				[Updated_By],
				[Updated_Date],
				Is_Deleted
		)
		VALUES
		(
				@Role_Id,
				@Claim_Id,
				@Created_By,
				GETDATE(),
				@Created_By,
				GETDATE(),
				0
		)
		SELECT
			SCOPE_IDENTITY() AS [Id]
	 END
END