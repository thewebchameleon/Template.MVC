--add system user to database
DELETE FROM [User]
DBCC CHECKIDENT('[User]', RESEED, 0)

INSERT INTO [User]
VALUES ('system', 'test@example.com', 1, 'Sys', 'Tem', NULL, NULL, 0, NULL, 1, NULL, GETDATE(), NULL, GETDATE(), 0)

INSERT INTO [Role]
([Name], [Description], [Is_Enabled], Created_By, Created_Date, Updated_By, Updated_Date, Is_Deleted)
VALUES ('Admin', 'Administrator account', 1, 1, GETDATE(), 1, GETDATE(), 0)

INSERT INTO [Claim]
([Type], [Value], Created_By, Created_Date, Updated_By, Updated_Date, Is_Deleted)
VALUES ('Admin', 'Create and update users', 1, GETDATE(), 1, GETDATE(), 0)

INSERT INTO [Claim]
([Type], [Value], Created_By, Created_Date, Updated_By, Updated_Date, Is_Deleted)
VALUES ('Admin', 'Disable and enable users', 1, GETDATE(), 1, GETDATE(), 0)