DECLARE @dbName VarChar(50)
DECLARE @autoDrop bit
--NOTE FROM MATT: In order for this script to be able to accept values
--from the installer, it has to use SQLCMD scripting variables.  This
--will be reset back to 'SchemaManagerTest' if the variable isn't defined.
SET @dbName = '$(DBNAME)'
DECLARE @commandString NVarChar(Max)
SET NOCOUNT ON

--This fixes the script so it runs in SSMS.  The variable
--is broken into two strings so that it won't be substituted
--when run from the SQLCMD utility.
if @dbName = '$' + '(DBNAME)'
	SET @dbName = 'SchemaManagerTest'

--Remove database if already exists
IF EXISTS (SELECT * FROM sysdatabases WHERE name = @dbName)
BEGIN
	EXEC('ALTER DATABASE [' + @dbName + ' ] SET SINGLE_USER WITH ROLLBACK IMMEDIATE')
	EXEC('DROP DATABASE [' + @dbName + ']')
END

--Create database file
SET @commandString = 'CREATE DATABASE [' + @dbName + ']'
EXEC sp_executesql @commandString
 
--Compatibility level
EXEC dbo.sp_dbcmptlevel @dbname=@dbName, @new_cmptlevel=90
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET ANSI_NULL_DEFAULT OFF'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET ANSI_NULLS OFF'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET ANSI_PADDING OFF'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET ANSI_WARNINGS OFF'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET ARITHABORT OFF'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET AUTO_CLOSE OFF'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET AUTO_CREATE_STATISTICS ON'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET AUTO_SHRINK ON'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET AUTO_UPDATE_STATISTICS ON'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET CURSOR_CLOSE_ON_COMMIT OFF'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET CURSOR_DEFAULT  GLOBAL'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET CONCAT_NULL_YIELDS_NULL OFF'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET NUMERIC_ROUNDABORT OFF'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET QUOTED_IDENTIFIER OFF'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET RECURSIVE_TRIGGERS OFF'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET AUTO_UPDATE_STATISTICS_ASYNC ON'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET DATE_CORRELATION_OPTIMIZATION OFF'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET  READ_WRITE'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET RECOVERY SIMPLE'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET  MULTI_USER'
EXEC sp_executesql @commandString
 
SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET PAGE_VERIFY CHECKSUM'
EXEC sp_executesql @commandString

SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET DB_CHAINING OFF'
EXEC sp_executesql @commandString

SET @commandString = 'ALTER DATABASE [' + @dbName + '] SET TRUSTWORTHY ON'
EXEC sp_executesql @commandString

EXEC('EXEC sp_configure ''clr enabled'', 1')
EXEC('RECONFIGURE WITH OVERRIDE')

SET @commandString = 'IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N''SchemaManagerTestUser'')  CREATE LOGIN [SchemaManagerTestUser] WITH PASSWORD=N''SchemaManagerInt01'', DEFAULT_DATABASE=[' + @dbName + '], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF'
EXEC sp_executesql @commandString

SET @commandString = 'USE [' + @dbName + ']; IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N''SchemaManagerTestUser'')  CREATE USER [SchemaManagerTestUser] FOR LOGIN [SchemaManagerTestUser] WITH DEFAULT_SCHEMA=[dbo]'
EXEC sp_executesql @commandString

SET @commandString = 'USE [' + @dbName + ']; IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N''SchemaManagerTestUser'')  EXEC sys.sp_executesql N''CREATE SCHEMA [SchemaManagerTestUser] AUTHORIZATION [SchemaManagerTestUser]'''
EXEC sp_executesql @commandString


SET @commandString = 'USE [' + @dbName + ']; GRANT EXECUTE TO SchemaManagerTestUser'
EXEC sp_executesql @commandString

SET @commandString = 'USE [' + @dbName + ']; EXEC sp_addrolemember ''db_owner'', ''SchemaManagerTestUser'''
EXEC sp_executesql @commandString

SET @commandString = 'USE [' + @dbName + ']; ALTER DATABASE [' + @dbName + '] SET RECURSIVE_TRIGGERS ON'
EXEC sp_executesql @commandString

EXEC sp_password @loginame=N'SchemaManagerTestUser', @new=N'SchemaManagerInt01'

ScriptEnd: