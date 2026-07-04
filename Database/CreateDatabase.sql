-- Local Supermarket Management System — SQL Server creation script
-- Run with:  sqlcmd -S ".\SQLEXPRESS" -i Database/CreateDatabase.sql
-- NOTE: The application also creates this schema automatically via Entity Framework Core
-- on first run. This script is provided as a stand-alone deliverable.

IF DB_ID('SupermarketDb') IS NULL
    CREATE DATABASE SupermarketDb;
GO
USE SupermarketDb;
GO

IF OBJECT_ID('dbo.SaleItems') IS NOT NULL DROP TABLE dbo.SaleItems;
IF OBJECT_ID('dbo.Sales') IS NOT NULL DROP TABLE dbo.Sales;
IF OBJECT_ID('dbo.StockMovements') IS NOT NULL DROP TABLE dbo.StockMovements;
IF OBJECT_ID('dbo.Products') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Suppliers') IS NOT NULL DROP TABLE dbo.Suppliers;
IF OBJECT_ID('dbo.Categories') IS NOT NULL DROP TABLE dbo.Categories;
GO

CREATE TABLE dbo.Categories (
    Id   INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.Suppliers (
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Name    NVARCHAR(100) NOT NULL,
    Contact NVARCHAR(100) NULL
);

CREATE TABLE dbo.Products (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(150) NOT NULL,
    Brand       NVARCHAR(100) NULL,
    Barcode     NVARCHAR(50)  NOT NULL,
    Price       DECIMAL(10,2) NOT NULL,
    Quantity    INT           NOT NULL,
    ExpiryDate  DATETIME2     NULL,
    RestockDate DATETIME2     NULL,
    CategoryId  INT NOT NULL CONSTRAINT FK_Products_Categories REFERENCES dbo.Categories(Id),
    SupplierId  INT NOT NULL CONSTRAINT FK_Products_Suppliers  REFERENCES dbo.Suppliers(Id)
);
CREATE UNIQUE INDEX IX_Products_Barcode ON dbo.Products(Barcode);

CREATE TABLE dbo.StockMovements (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL CONSTRAINT FK_StockMovements_Products REFERENCES dbo.Products(Id),
    Change    INT NOT NULL,
    Reason    NVARCHAR(100) NOT NULL,
    Date      DATETIME2 NOT NULL
);

CREATE TABLE dbo.Sales (
    Id    INT IDENTITY(1,1) PRIMARY KEY,
    Date  DATETIME2 NOT NULL,
    Total DECIMAL(10,2) NOT NULL
);

CREATE TABLE dbo.SaleItems (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    SaleId    INT NOT NULL CONSTRAINT FK_SaleItems_Sales    REFERENCES dbo.Sales(Id),
    ProductId INT NOT NULL CONSTRAINT FK_SaleItems_Products REFERENCES dbo.Products(Id),
    Quantity  INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL
);
GO

-- Sample data ---------------------------------------------------------------
INSERT INTO dbo.Categories (Name) VALUES (N'Drinks'), (N'Bakery'), (N'Dairy'), (N'Produce');
INSERT INTO dbo.Suppliers (Name, Contact) VALUES
    (N'Metro Cash & Carry', N'orders@metro-cc.com'), (N'DMART', N'sales@dmart.com');

INSERT INTO dbo.Products (Name, Brand, Barcode, Price, Quantity, ExpiryDate, CategoryId, SupplierId) VALUES
    (N'Cola 1L',         N'FizzCo',    N'1000001', 1.20, 40, DATEADD(month, 8, GETDATE()),  1, 2),
    (N'Orange Juice 1L', N'Sunny',     N'1000002', 1.80,  3, DATEADD(day, 20, GETDATE()),   1, 1),
    (N'White Bread',     N'BakeHouse', N'1000003', 0.95, 25, DATEADD(day, 5, GETDATE()),    2, 2),
    (N'Croissant',       N'BakeHouse', N'1000004', 0.70,  2, DATEADD(day, 3, GETDATE()),    2, 2),
    (N'Whole Milk 2L',   N'FarmFresh', N'1000005', 1.40, 30, DATEADD(day, 10, GETDATE()),   3, 1),
    (N'Cheddar 200g',    N'FarmFresh', N'1000006', 2.50, 15, DATEADD(month, 2, GETDATE()),  3, 1),
    (N'Bananas 1kg',     N'FreshPick', N'1000007', 1.10,  4, DATEADD(day, 7, GETDATE()),    4, 1),
    (N'Apples 1kg',      N'FreshPick', N'1000008', 1.60, 50, DATEADD(day, 14, GETDATE()),   4, 1);
GO
