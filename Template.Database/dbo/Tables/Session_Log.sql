﻿CREATE TABLE [dbo].[Session_Log] (
    [Id]				INT          IDENTITY (1, 1) NOT NULL,
    [Session_Id]		INT          NOT NULL,
	[Method]			VARCHAR(10)  NOT NULL,
	[Controller]		VARCHAR(256) NOT NULL,
	[Action]			VARCHAR(256) NOT NULL,
    [Created_By]		INT          NOT NULL,
    [Created_Date]		DATETIME     NOT NULL,
    [Updated_By]		INT          NOT NULL,
    [Updated_Date]		DATETIME     NOT NULL,
    [Is_Deleted]		BIT          NOT NULL,
    CONSTRAINT [PK_Session_Log] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Session_Log__User_Created_By] FOREIGN KEY ([Created_By]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Session_Log__User_Updated_By] FOREIGN KEY ([Updated_By]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Session_Log__Session_Session_Id] FOREIGN KEY ([Session_Id]) REFERENCES [dbo].[Session] ([Id]),
);

GO
CREATE NONCLUSTERED INDEX [IX_Session_Log_Is_Deleted]
    ON [dbo].[Session_Log]([Is_Deleted] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Session_Log_Session_Id]
    ON [dbo].[Session_Log]([Session_Id] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Session_Log_Controller]
    ON [dbo].[Session_Log]([Controller] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Session_Log_Action]
    ON [dbo].[Session_Log]([Action] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_Session_Log_Created_Date]
    ON [dbo].[Session_Log]([Created_Date] ASC);
GO