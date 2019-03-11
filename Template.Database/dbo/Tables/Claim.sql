CREATE TABLE [dbo].[Claim] (
    [Id]				INT IDENTITY (1, 1) NOT NULL,
    [Type]				VARCHAR (256)		NOT NULL,
    [Value]				VARCHAR (256)		NULL,
    [Created_By]		INT					NOT NULL,
    [Created_Date]		DATETIME			NOT NULL,
    [Updated_By]		INT					NOT NULL,
    [Updated_Date]		DATETIME			NOT NULL,
    [Is_Deleted]		BIT					NOT NULL,
    CONSTRAINT [PK_Claim] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Claim__User_Created_By] FOREIGN KEY ([Created_By]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Claim__User_Updated_By] FOREIGN KEY ([Updated_By]) REFERENCES [dbo].[User] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Claim_Is_Deleted] 
	ON [dbo].[Claim] ([Is_Deleted])
GO