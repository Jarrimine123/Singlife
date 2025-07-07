CREATE TABLE JourneyMapping
(
    MilestoneID INT IDENTITY(1,1) PRIMARY KEY,
    AccountID INT NOT NULL,
    Year INT NOT NULL,
    Title NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255),
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ModifiedDate DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    DeleteDate DATETIME NULL,


);
