CREATE TABLE [dbo].[User_Claim] (
    [Id]           INT      IDENTITY (1, 1) NOT NULL,
    [User_Id]      INT      NOT NULL,
    [Claim_Id]      INT      NOT NULL,
    [Created_By]   INT      NOT NULL,
    [Created_Date] DATETIME NOT NULL,
    [Updated_By]   INT      NOT NULL,
    [Updated_Date] DATETIME NOT NULL,
    [Is_Deleted]   BIT      NOT NULL,
    CONSTRAINT [PK_User_Claim] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User_Claim__User_Created_By] FOREIGN KEY ([Created_By]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_User_Claim__User_Updated_By] FOREIGN KEY ([Updated_By]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_User_Claim_Claim] FOREIGN KEY ([Claim_Id]) REFERENCES [dbo].[Claim] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_User_Claim_Is_Deleted]
	ON [dbo].[User_Claim]([Is_Deleted] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_User_Claim_User_Id]
	ON [dbo].[User_Claim]([User_Id] ASC)
GO