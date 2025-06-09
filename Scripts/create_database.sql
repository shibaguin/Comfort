-- Переключаемся на базу данных User27
USE User27;
GO

-- Создаём таблицу ProductTypes (Типы продукции)
CREATE TABLE ProductTypes (
    ProductTypeID INT PRIMARY KEY IDENTITY(1,1),
    ProductTypeName NVARCHAR(100) UNIQUE NOT NULL,
    Coefficient DECIMAL(5,2) NOT NULL
);
GO

-- Создаём таблицу Materials (Материалы)
CREATE TABLE Materials (
    MaterialID INT PRIMARY KEY IDENTITY(1,1),
    MaterialName NVARCHAR(100) UNIQUE NOT NULL,
    LossPercentage DECIMAL(5,2) NOT NULL
);
GO

-- Создаём таблицу Workshops (Цехи)
CREATE TABLE Workshops (
    WorkshopID INT PRIMARY KEY IDENTITY(1,1),
    WorkshopName NVARCHAR(100) UNIQUE NOT NULL,
    WorkshopType NVARCHAR(100) NOT NULL,
    StaffCount INT NOT NULL
);
GO

-- Создаём таблицу Products (Продукция)
CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    Article NVARCHAR(50) UNIQUE NOT NULL,
    ProductTypeID INT NOT NULL,
    ProductName NVARCHAR(100) NOT NULL,
    MinPartnerCost DECIMAL(18,2) NOT NULL,
    MainMaterialID INT NOT NULL,
    FOREIGN KEY (ProductTypeID) REFERENCES ProductTypes(ProductTypeID),
    FOREIGN KEY (MainMaterialID) REFERENCES Materials(MaterialID)
);
GO

-- Создаём таблицу ProductWorkshops (Связь продукции и цехов)
CREATE TABLE ProductWorkshops (
    ProductWorkshopID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT NOT NULL,
    WorkshopID INT NOT NULL,
    ManufacturingTime DECIMAL(5,2) NOT NULL,
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (WorkshopID) REFERENCES Workshops(WorkshopID)
);
GO