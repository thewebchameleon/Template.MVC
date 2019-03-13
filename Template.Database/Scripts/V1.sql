--add system user to database
DELETE FROM [User]
DBCC CHECKIDENT('[User]', RESEED, 0)

INSERT INTO [User]
VALUES ('system', 'test@example.com', 1, 'Sys', 'Tem', NULL, NULL, 0, NULL, 1, NULL, GETDATE(), NULL, GETDATE(), 0)