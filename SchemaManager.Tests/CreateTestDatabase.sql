USE [master]
GO


CREATE DATABASE [SchemaManagerIntegrationTests]
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET COMPATIBILITY_LEVEL = 100
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET ARITHABORT OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET  DISABLE_BROKER 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET  READ_WRITE 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET RECOVERY FULL 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET  MULTI_USER 
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [SchemaManagerIntegrationTests] SET DB_CHAINING OFF 
GO


CREATE LOGIN [SchemaManagerTestUser] WITH PASSWORD=N'SchemaManagerTestUser', DEFAULT_DATABASE=[SchemaManagerIntegrationTests], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

ALTER LOGIN [SchemaManagerTestUser] ENABLE
GO

USE [SchemaManagerIntegrationTests]
GO

CREATE USER [SchemaManagerTestUser] FOR LOGIN [SchemaManagerTestUser] WITH DEFAULT_SCHEMA=[dbo]
GO

EXEC sp_addrolemember N'db_owner', N'SchemaManagerTestUser' 
