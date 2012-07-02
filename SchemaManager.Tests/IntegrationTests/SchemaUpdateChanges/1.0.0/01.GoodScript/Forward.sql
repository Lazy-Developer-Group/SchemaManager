CREATE TABLE Logs
(
	ID uniqueidentifier,
	Content VARCHAR(50)
)
GO

CREATE PROCEDURE [dbo].[AddLogMsg]
	@Content VARCHAR(50)
AS
BEGIN

DECLARE @RETVAL int
SET @RETVAL = 0	

	BEGIN TRY
		BEGIN TRANSACTION
		
		INSERT INTO Logs (Content)
		VALUES (@Content)
		SET @RETVAL = (SELECT @@ROWCOUNT + @RETVAL) --should equal 1 at this point 
		
		IF @RETVAL = 1 COMMIT TRANSACTION
		ELSE ROLLBACK TRANSACTION
		
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SELECT -1 AS ReturnValue -- Return -1 if anything failed
	END CATCH

END