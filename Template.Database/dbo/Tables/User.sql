﻿CREATE TABLE [dbo].[User] (
    [Id]							INT            IDENTITY (1, 1) NOT NULL,
    [Username]						VARCHAR (256)  NOT NULL,
    [Email_Address]					VARCHAR (256)  NOT NULL,
    [Registration_Confirmed]		BIT    NOT NULL,
    [First_Name]					VARCHAR (100)  NULL,
    [Last_Name]						VARCHAR (100)  NULL,
    [Mobile_Number]					VARCHAR (30)   NULL,
    [Password_Hash]					VARCHAR (MAX)  NULL,
    [Is_Locked_Out]					BIT            NOT NULL,
    [Lockout_End]					DATETIME       NULL,
	[Is_Enabled]					BIT				NOT NULL,
    [Created_By]					INT            NULL,
    [Created_Date]					DATETIME       NOT NULL,
    [Updated_By]					INT            NULL,
    [Updated_Date]					DATETIME       NOT NULL,
    [Is_Deleted]					BIT            NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User__User_Created_By] FOREIGN KEY ([Created_By]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_User__User_Updated_By] FOREIGN KEY ([Updated_By]) REFERENCES [dbo].[User] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_User_Is_Deleted]
    ON [dbo].[User]([Is_Deleted] ASC);
GO