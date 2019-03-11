CREATE PROCEDURE [dbo].[sp_users_get]
AS
     BEGIN
         SELECT [Id],
                [Username],
                [Email_Address],
                [Registration_Confirmed],
                [First_Name],
                [Last_Name],
                [Mobile_Number],
                [Password_Hash],
                [Is_Locked_Out],
                [Lockout_End],
                [Created_By],
                [Created_Date],
                [Updated_By],
                [Updated_Date],
                Is_Deleted
         FROM [User](NOLOCK)
         WHERE Is_Deleted = 0;
     END;