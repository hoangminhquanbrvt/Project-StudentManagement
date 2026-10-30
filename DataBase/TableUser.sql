USE StudentDB
GO
CREATE TABLE Users
(
    Username VARCHAR(50) PRIMARY KEY,
    Password VARCHAR(50) NOT NULL
);
GO

INSERT INTO Users (Username, Password)
VALUES ('admin', '123');
GO