-- Switch to the master database before dropping the target database

USE master;
GO
 
-- Force drop the database if it exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'prs_db')
BEGIN
    ALTER DATABASE prs_db SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE prs_db;
END
GO
 
-- Create the database
CREATE DATABASE prs_db;
GO
 
-- Switch to the new database
USE prs_db;
GO

-- Drop tables if they exist to avoid duplication errors
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE Users;
GO
IF OBJECT_ID('dbo.Vendor', 'U') IS NOT NULL DROP TABLE Vendor;
GO
IF OBJECT_ID('dbo.Product', 'U') IS NOT NULL DROP TABLE Product;
GO
IF OBJECT_ID('dbo.Request', 'U') IS NOT NULL DROP TABLE Request;
GO
IF OBJECT_ID('dbo.LineItem', 'U') IS NOT NULL DROP TABLE LineItem;
GO
 
-- Create Users Table
CREATE TABLE Users (
    ID			INT				PRIMARY KEY IDENTITY(1,1),
    Username	VARCHAR(20)		NOT NULL,
    Password	VARCHAR(255)	NOT NULL, -- Increased length for security (hashed passwords)
    FirstName	VARCHAR(20)		NOT NULL,
    LastName	VARCHAR(20)		NOT NULL,
    PhoneNumber VARCHAR(15)		NOT NULL,
    Email		VARCHAR(75)		NOT NULL,
    Reviewer	BIT				NOT NULL DEFAULT 0,
    Admin		BIT				NOT NULL DEFAULT 0,
    CONSTRAINT UC_UName UNIQUE (Username)
);

GO

-- Create Vendor Table
CREATE TABLE Vendor (
    ID			INT				PRIMARY KEY IDENTITY(1,1),
    Code		VARCHAR(10)		NOT NULL,
    Name		VARCHAR(255)	NOT NULL,
    Address		VARCHAR(255)	NOT NULL,
    City		VARCHAR(255)	NOT NULL,
    State		VARCHAR(2)		NOT NULL,
    Zip			VARCHAR(10)		NOT NULL, -- Supports international zip codes
    PhoneNumber VARCHAR(15)		NOT NULL,
    Email		VARCHAR(100)	NOT NULL,
    CONSTRAINT UC_VCode UNIQUE (Code)
);
GO
 
-- Create Product Table
CREATE TABLE Product (
    ID				INT				PRIMARY KEY IDENTITY(1,1),
    VendorID		INT				NOT NULL,
    PartNumber		VARCHAR(50)		NOT NULL,
    Name			VARCHAR(150)	NOT NULL,
    Price			DECIMAL(10, 2)	NOT NULL CHECK (Price > 0),
    Unit			VARCHAR(255)		NULL,
    PhotoPath		VARCHAR(255)		NULL,
    FOREIGN KEY (VendorID) REFERENCES Vendor(ID) ON DELETE CASCADE,
    CONSTRAINT UC_Vendor_Part UNIQUE (VendorID, PartNumber)
);
GO
 
-- Create Request Table
CREATE TABLE Request (
    ID				INT				PRIMARY KEY IDENTITY(1,1),
    UserID			INT				NOT NULL,
    RequestNumber	VARCHAR(20)		NOT NULL,
    Description		VARCHAR(100)	NOT NULL,
    Justification	VARCHAR(255)	NOT NULL,
    DateNeeded		DATE			NOT NULL,
    DeliveryMode	VARCHAR(25)		NOT NULL CHECK (DeliveryMode IN ('Mail', 'Pickup')),
    STATUS			VARCHAR(20)		NOT NULL DEFAULT 'NEW' CHECK (STATUS IN ('NEW', 'REVIEW', 'APPROVED', 'REJECTED')),
    TOTAL			DECIMAL(10,2)	NOT NULL DEFAULT 0.0 CHECK (TOTAL >= 0),
    SubmittedDate	DATETIME		NOT NULL DEFAULT GETDATE(),
    ReasonForRejection	VARCHAR(100) NULL,
    FOREIGN KEY (UserID) REFERENCES Users(ID) ON DELETE CASCADE
);
GO
 
-- Create LineItem Table
CREATE TABLE LineItem (
    ID				INT				PRIMARY KEY IDENTITY(1,1),
    RequestID		INT				NOT NULL,
    ProductID		INT				NOT NULL,
    Quantity		INT				NOT NULL CHECK (Quantity > 0),
    FOREIGN KEY (ProductID) REFERENCES Product(ID) ON DELETE CASCADE,
    FOREIGN KEY (RequestID) REFERENCES Request(ID) ON DELETE CASCADE,
    CONSTRAINT UC_Req_Pdt UNIQUE (RequestID, ProductID)
);
GO
 