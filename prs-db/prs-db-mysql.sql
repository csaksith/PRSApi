 
-- Force drop the database if it exists
DROP DATABASE IF EXISTS prs_db;

-- Create the database
CREATE DATABASE prs_db;

-- Switch to the new database
USE prs_db;

-- Drop tables if they exist to avoid duplication errors
DROP TABLE IF EXISTS User;
DROP TABLE IF EXISTS Vendor;
DROP TABLE IF EXISTS Product;
DROP TABLE IF EXISTS Request;
DROP TABLE IF EXISTS LineItem;

-- Create User Table
CREATE TABLE User (
    ID			INT				PRIMARY KEY AUTO_INCREMENT,
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

-- Create Vendor Table
CREATE TABLE Vendor (
    ID			INT				PRIMARY KEY AUTO_INCREMENT,
    Code		VARCHAR(10)		NOT NULL,
    Name		VARCHAR(255)	NOT NULL,
    Address		VARCHAR(255)	NOT NULL,
    City		VARCHAR(255)	NOT NULL,
    State		VARCHAR(2)		NOT NULL,
    Zip			VARCHAR(10)		NOT NULL, 
    PhoneNumber VARCHAR(15)		NOT NULL,
    Email		VARCHAR(100)	NOT NULL,
    CONSTRAINT UC_VCode UNIQUE (Code)
);
 
-- Create Product Table
CREATE TABLE Product (
    ID				INT				PRIMARY KEY AUTO_INCREMENT,
    VendorID		INT				NOT NULL,
    PartNumber		VARCHAR(50)		NOT NULL,
    Name			VARCHAR(150)	NOT NULL,
    Price			DECIMAL(10, 2)	NOT NULL CHECK (Price > 0),
    Unit			VARCHAR(255)		NULL,
    PhotoPath		VARCHAR(255)		NULL,
    FOREIGN KEY (VendorID) REFERENCES Vendor(ID) ON DELETE CASCADE,
    CONSTRAINT UC_Vendor_Part UNIQUE (VendorID, PartNumber)
);

-- Create Request Table
CREATE TABLE Request (
    ID				INT				PRIMARY KEY AUTO_INCREMENT,
    UserID			INT				NOT NULL,
    RequestNumber	VARCHAR(20)		NOT NULL,
    Description		VARCHAR(100)	NOT NULL,
    Justification	VARCHAR(255)	NOT NULL,
    DateNeeded		DATE			NOT NULL,
    DeliveryMode	VARCHAR(25)		NOT NULL CHECK (DeliveryMode IN ('Mail', 'Pickup')),
    STATUS			VARCHAR(20)		NOT NULL DEFAULT 'NEW' CHECK (STATUS IN ('NEW', 'REVIEW', 'APPROVED', 'REJECTED')),
    TOTAL			DECIMAL(10,2)	NOT NULL DEFAULT 0.0 CHECK (TOTAL >= 0),
    SubmittedDate	DATETIME		NOT NULL DEFAULT NOW(),
    ReasonForRejection	VARCHAR(100) NULL,
    FOREIGN KEY (UserID) REFERENCES User(ID) ON DELETE CASCADE
);

 
-- Create LineItem Table
CREATE TABLE LineItem (
    ID				INT				PRIMARY KEY AUTO_INCREMENT,
    RequestID		INT				NOT NULL,
    ProductID		INT				NOT NULL,
    Quantity		INT				NOT NULL CHECK (Quantity > 0),
    FOREIGN KEY (ProductID) REFERENCES Product(ID) ON DELETE CASCADE,
    FOREIGN KEY (RequestID) REFERENCES Request(ID) ON DELETE CASCADE,
    CONSTRAINT UC_Req_Pdt UNIQUE (RequestID, ProductID)
);

 