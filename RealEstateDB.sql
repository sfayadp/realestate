CREATE DATABASE RealEstateDB;
GO
USE RealEstateDB;
GO

IF OBJECT_ID('dbo.PropertyTrace', 'U') IS NOT NULL DROP TABLE dbo.PropertyTrace;
IF OBJECT_ID('dbo.PropertyImage', 'U') IS NOT NULL DROP TABLE dbo.PropertyImage;
IF OBJECT_ID('dbo.Property', 'U') IS NOT NULL DROP TABLE dbo.Property;
IF OBJECT_ID('dbo.Owner', 'U') IS NOT NULL DROP TABLE dbo.Owner;
GO

CREATE TABLE dbo.Owner (
    IdOwner   INT IDENTITY(1,1) NOT NULL,
    [Name]    NVARCHAR(150)     NOT NULL,
    [Address] NVARCHAR(250)     NULL,
    Photo     VARBINARY(MAX)    NULL,
    Birthday  DATE              NULL,
    CONSTRAINT PK_Owner PRIMARY KEY CLUSTERED (IdOwner)
);
GO

CREATE TABLE dbo.[Property] (
    IdProperty   INT IDENTITY(1,1) NOT NULL,
    [Name]       NVARCHAR(150)     NOT NULL,
    [Address]    NVARCHAR(250)     NULL,
    Price        DECIMAL(18,2)     NOT NULL,
    CodeInternal NVARCHAR(50)      NOT NULL,
    [Year]       SMALLINT          NULL,
    IdOwner      INT               NOT NULL,
    CONSTRAINT PK_Property PRIMARY KEY CLUSTERED (IdProperty),
    CONSTRAINT UQ_Property_CodeInternal UNIQUE (CodeInternal),
    CONSTRAINT CK_Property_Price_Positive CHECK (Price >= 0),
    CONSTRAINT CK_Property_Year CHECK ([Year] IS NULL OR ([Year] BETWEEN 1800 AND YEAR(GETDATE())+1)),
    CONSTRAINT FK_Property_Owner FOREIGN KEY (IdOwner) REFERENCES dbo.Owner(IdOwner) ON UPDATE NO ACTION ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Property_IdOwner ON dbo.[Property](IdOwner);
GO

CREATE TABLE dbo.PropertyImage (
    IdPropertyImage INT IDENTITY(1,1) NOT NULL,
    IdProperty      INT               NOT NULL,
    [File]          VARBINARY(MAX)    NOT NULL,
    Enabled         BIT               NOT NULL CONSTRAINT DF_PropertyImage_Enabled DEFAULT (1),
    CONSTRAINT PK_PropertyImage PRIMARY KEY CLUSTERED (IdPropertyImage),
    CONSTRAINT FK_PropertyImage_Property FOREIGN KEY (IdProperty) REFERENCES dbo.[Property](IdProperty) ON UPDATE NO ACTION ON DELETE NO ACTION
);
GO

CREATE INDEX IX_PropertyImage_IdProperty ON dbo.PropertyImage(IdProperty);
GO

CREATE TABLE dbo.PropertyTrace (
    IdPropertyTrace INT IDENTITY(1,1) NOT NULL,
    DateSale        DATE              NOT NULL,
    [Name]          NVARCHAR(150)     NOT NULL,
    [Value]         DECIMAL(18,2)     NOT NULL,
    Tax             DECIMAL(18,2)     NOT NULL,
    IdProperty      INT               NOT NULL,
    CONSTRAINT PK_PropertyTrace PRIMARY KEY CLUSTERED (IdPropertyTrace),
    CONSTRAINT CK_PropertyTrace_Value_Positive CHECK ([Value] >= 0),
    CONSTRAINT CK_PropertyTrace_Tax_Positive CHECK (Tax >= 0),
    CONSTRAINT FK_PropertyTrace_Property FOREIGN KEY (IdProperty) REFERENCES dbo.[Property](IdProperty) ON UPDATE NO ACTION ON DELETE NO ACTION
);
GO

CREATE INDEX IX_PropertyTrace_IdProperty_Date ON dbo.PropertyTrace(IdProperty, DateSale);
GO
