USE [master]
GO

EXEC sp_configure filestream_access_level, 2
GO
RECONFIGURE
GO

IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'audio_db') BEGIN
ALTER DATABASE audio_db SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE audio_db;
END;
CREATE DATABASE audio_db
WITH FILESTREAM
( 
NON_TRANSACTED_ACCESS = FULL,
DIRECTORY_NAME = N'audio_store'
);
GO

ALTER DATABASE audio_db
ADD FILEGROUP audio_db_filegroup
CONTAINS FILESTREAM;
GO

ALTER DATABASE audio_db
ADD FILE
(
NAME= 'audio_db_store',
FILENAME = 'C:\audio_store\'
)
TO FILEGROUP audio_db_filegroup;
GO

USE [audio_db];
GO
CREATE TABLE AudioStore AS FILETABLE
WITH
( 
FILETABLE_DIRECTORY = 'AudioStore',
FILETABLE_COLLATE_FILENAME = database_default
);
GO