CREATE TABLE [dbo].[Email_Template] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Key]          VARCHAR (256) NOT NULL,
    [Description]  VARCHAR (256) NOT NULL,
    [Body]		   VARCHAR (MAX) NULL,
    [Created_By]   INT           NOT NULL,
    [Created_Date] DATETIME      NOT NULL,
    [Updated_By]   INT           NOT NULL,
    [Updated_Date] DATETIME      NOT NULL,
    [Is_Deleted]   BIT           CONSTRAINT [DF_Email_Template_Is_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Email_Template] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Email_Template__User_Created_By] FOREIGN KEY ([Created_By]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Email_Template__User_Updated_By] FOREIGN KEY ([Updated_By]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [UC_Email_Template__Key] UNIQUE NONCLUSTERED ([Key] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_Email_Template_Is_Deleted] 
	ON [dbo].[Email_Template] ([Is_Deleted])
GO