
INSERT INTO Logs (Content)
VALUES ('Test log from script 2')

GO

EXEC [AddLogMsg] 'Test log via sproc.'

GO

--This should blow up
CREATE TABLE Logs
(
	ID uniqueidentifier,
	Content VARCHAR(50)
)

GO

INSERT INTO Logs (Content)
VALUES ('Another test log from script 2')

