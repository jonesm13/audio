USE [master]
GO

EXEC sp_configure filestream_access_level, 2
GO
RECONFIGURE
GO

ALTER DATABASE [audio_db] ADD FILEGROUP [audio_files_group] CONTAINS FILESTREAM
GO
ALTER DATABASE [audio_db]
    ADD FILE (
        NAME = N'audio_files',
        FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\audio_files')
    TO FILEGROUP [audio_files_group]
GO

USE [audio_db]
GO

CREATE TABLE dbo.AudioFile
(
    [Id] [uniqueidentifier] ROWGUIDCOL NOT NULL UNIQUE,
    [Data] VARBINARY(MAX) FILESTREAM NULL
)
GO