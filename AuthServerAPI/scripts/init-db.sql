CREATE DATABASE AuthServer;
GO

USE AuthServer;
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

INSERT INTO Users (Username, Email) VALUES ('testuser', 'test@example.com');
GO