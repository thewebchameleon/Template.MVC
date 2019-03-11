CREATE TABLE [dbo].[Session] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [Guid]        VARCHAR (60) NOT NULL,
    [User_Id]      INT          NULL,
    [Created_By]   INT          NOT NULL,
    [Created_Date] DATETIME     NOT NULL,
    [Updated_By]   INT          NOT NULL,
    [Updated_Date] DATETIME     NOT NULL,
    [Is_Deleted]     BIT          NOT NULL,
    CONSTRAINT [PK_Session] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Session__User_Created_By] FOREIGN KEY ([Created_By]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Session__User_Updated_By] FOREIGN KEY ([Updated_By]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Session__User_UserId] FOREIGN KEY ([User_Id]) REFERENCES [dbo].[User] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_Session_Is_Deleted]
    ON [dbo].[Session]([Is_Deleted] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Session_Guid]
    ON [dbo].[Session]([Guid] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Session_UserId]
    ON [dbo].[Session]([User_Id] ASC);
GO