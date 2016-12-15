CREATE TABLE Users
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NULL, 
    [LastName] NVARCHAR(50) NULL, 
    [LastAccessDate] DATETIME NULL, 
    [AccessIP] NVARCHAR(50) NULL, 
    [AccessLocation] NVARCHAR(50) NULL, 
    [AccessedTargetID] INT NULL
)
