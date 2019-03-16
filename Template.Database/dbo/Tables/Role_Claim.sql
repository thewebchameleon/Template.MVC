CREATE TABLE [dbo].[Role_Claim] (
    [Id]           INT      IDENTITY (1, 1) NOT NULL,
    [Role_Id]      INT      NOT NULL,
    [Claim_Id]     INT      NOT NULL,
    [Created_By]   INT      NOT NULL,
    [Created_Date] DATETIME NOT NULL,
    [Updated_By]   INT      NOT NULL,
    [Updated_Date] DATETIME NOT NULL,
    [Is_Deleted]   BIT      NOT NULL,
    CONSTRAINT [PK_Role_Claim] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Role_Claim__Role_Role_Id] FOREIGN KEY ([Role_Id]) REFERENCES [dbo].[Role] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Role_Claim__User_Created_By] FOREIGN KEY ([Created_By]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Role_Claim__User_Updated_By] FOREIGN KEY ([Updated_By]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Role_Claim__Claim_Claim_Id] FOREIGN KEY ([Claim_Id]) REFERENCES [dbo].[Claim] ([Id]), 
	CONSTRAINT [UC_Role_Claim__Claim_Id_Role_Id] UNIQUE NONCLUSTERED (Claim_Id,Role_Id)
);
GO

CREATE NONCLUSTERED INDEX [IX_Role_Claim_Is_Deleted]
	ON [dbo].[Role_Claim]([Is_Deleted] ASC)
GO


