﻿CREATE PROCEDURE [dbo].[sp_user_disable]
	@User_Id    INT,
	@Updated_By INT
AS
BEGIN
   UPDATE [User]
   SET
		[Is_Enabled] = 0,
		[Updated_By] = @Updated_By,
		[Updated_Date] = GETDATE()
   WHERE
		[Id] = @User_Id
   SELECT
		@User_Id AS [Id]
END