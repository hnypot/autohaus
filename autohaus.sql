USE master
GO
IF EXISTS (SELECT * FROM sysdatabases WHERE name='autohaus')
BEGIN
RAISERROR('Die Datenbank "autohaus" existiert bereits, sie wird nun gelöscht... ', 0, 1)
ALTER DATABASE autohaus SET single_user WITH ROLLBACK IMMEDIATE
DROP DATABASE autohaus
END
CREATE DATABASE autohaus
GO
USE autohaus
GO

-- Geschrieben für das Projekt "Autohaus" (Modularbeit M133), von Alperen Yilmaz @ 2022

CREATE TABLE Benutzer
(
    Id INT NOT NULL IDENTITY(1,1),
    Benutzername NVARCHAR(30) NOT NULL,
    Passwort CHAR(64) NOT NULL, -- will be hashed as SHA256 via ComputeSha256Hash function in ASP.NET
	Admin BIT NOT NULL CONSTRAINT DF_Benutzer_Admin DEFAULT 0,
    CONSTRAINT PK_Benutzer_Id
        PRIMARY KEY (Id)
)

CREATE TABLE Kunden
(
    Id INT NOT NULL IDENTITY(1,1),
    BenutzerId INT NOT NULL,
    Vorname NVARCHAR(50) NOT NULL,
    Nachname NVARCHAR(50) NOT NULL,
    Email NVARCHAR(75),
    Telefon NVARCHAR(16),
    Strasse NVARCHAR(50),
    PLZ INT,
    Ort NVARCHAR(30),
	CONSTRAINT CHK_Kunden_PLZ CHECK (PLZ >= 4 OR PLZ <= 6),
    CONSTRAINT FK_Kunden_BenutzerId
        FOREIGN KEY (BenutzerId)
        REFERENCES Benutzer (Id)
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT PK_Kunden_Id
        PRIMARY KEY (Id)
)

CREATE TABLE Markt
(
    Id INT NOT NULL IDENTITY(1,1),
	Titel NVARCHAR(50) NOT NULL,
    Beschreibung NVARCHAR(MAX), --  sämtliche infos über das auto; auf marktseite begrenzen auf 10 zeichen o. ä., optional aber sehr empfohlen
    Preis MONEY NOT NULL,
    Erstellungsdatum DATETIME NOT NULL CONSTRAINT DF_Markt_DateTimeNow DEFAULT SYSDATETIME(),
    Verkauft BIT NOT NULL CONSTRAINT DF_Markt_Verkauft DEFAULT 0,
	CONSTRAINT CHK_Markt_Preis CHECK (Preis >= 0),
    CONSTRAINT PK_Markt_Id
        PRIMARY KEY (Id)
)

CREATE TABLE Vertrag
(
    Vertragnummer INT NOT NULL IDENTITY(1,1),
    MarktId INT NOT NULL,
	BenutzerId INT NOT NULL,
    Beschreibung NVARCHAR(MAX), -- optionale, zusätzliche infos bzw. beschreibung des vertrages
    Erstellungsdatum DATETIME CONSTRAINT DF_Vertrag_DateTimeNow DEFAULT SYSDATETIME(),
	Gueltig BIT NOT NULL CONSTRAINT DF_Vertrag_Gueltig DEFAULT 0,
    CONSTRAINT FK_Vertrag_MarktId
        FOREIGN KEY (MarktId)
        REFERENCES Markt (Id)
        ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT FK_Vertrag_BenutzerId
        FOREIGN KEY (BenutzerId)
        REFERENCES Benutzer (Id)
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT PK_Vertrag_Vertragnummer
        PRIMARY KEY (Vertragnummer)
)