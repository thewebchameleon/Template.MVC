CREATE PROCEDURE [dbo].[sp_user_tokens_get]
AS
     BEGIN
         SELECT [UT].[Id],
                [UT].[User_Id],
                [UT].[Guid],
                [UT].[Type_Id],
                [UT].[Expiry_Date],
                [UT].[Created_By],
                [UT].[Created_Date],
                [UT].[Updated_By],
                [UT].[Updated_Date],
                [UT].Is_Deleted
         FROM [User_Token] [UT](NOLOCK)
         WHERE UT.Is_Deleted = 0;
     END;