CREATE PROCEDURE [dbo].[sp_role_permission_create]
	@Role_Id     INT,
	@Permission_Id	INT,
	@Created_By  INT
AS
BEGIN
		-- find existing record else create a new one
	IF EXISTS
	(
		SELECT
				1
		FROM	[Role_Permission] (NOLOCK)
		WHERE	[Role_Id] = @Role_Id
		AND		[Permission_Id] = @Permission_Id
	)
	BEGIN

		-- reactivate deleted record
		DECLARE @Is_Deleted BIT = 0, 
				@Id INT;

		SELECT
			 @Is_Deleted = [Is_Deleted],
			 @Id = [Id]
		FROM   [Role_Permission] (NOLOCK)
		WHERE  [Role_Id] = @Role_Id
			 AND [Permission_Id] = @Permission_Id

		IF @Is_Deleted = 1
		BEGIN
			UPDATE [Role_Permission]
			SET 
				[Is_Deleted] = 0,
				Updated_By = Created_By,
				Updated_Date = GETDATE()
		END

		SELECT @Id AS [Id]
	END 
	ELSE
	BEGIN
		INSERT INTO [Role_Permission]
		(
				[Role_Id],
				[Permission_Id],
				[Created_By],
				[Created_Date],
				[Updated_By],
				[Updated_Date],
				Is_Deleted
		)
		VALUES
		(
				@Role_Id,
				@Permission_Id,
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