-- DATABASE CREATION SECTION
use master
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'EventEaseMSDB')
DROP DATABASE EventEaseMSDB
CREATE DATABASE EventEaseMSDB
use EventEaseMSDB

-- TABLE CREATION SECTION
CREATE TABLE Venue (
VenueID INT IDENTITY(1,1) PRIMARY KEY,
VenueName VARCHAR(150) UNIQUE NOT NULL,
[Location] VARCHAR(150) NOT NULL,
Capacity INT NOT NULL,
ImageURL VARCHAR(MAX) NOT NULL
);

CREATE TABLE [Event] (
EventID INT IDENTITY(1,1) PRIMARY KEY,
EventName VARCHAR(150) NOT NULL,
EventDate DATE NOT NULL,
[Description] VARCHAR(150) NOT NULL,
VenueID INT FOREIGN KEY (VenueID) REFERENCES Venue(VenueID)
);

CREATE TABLE Booking (
BookingID INT IDENTITY(1,1) PRIMARY KEY,
VenueID INT FOREIGN KEY (VenueID) REFERENCES Venue(VenueID),
EventID INT FOREIGN KEY (EventID) REFERENCES [Event](EventID),
BookingDate DATE NOT NULL,
CONSTRAINT UniqueBooking UNIQUE (VenueID, EventID, BookingDate)
);

-- TABLE INSERTION SECTION
INSERT INTO Venue (VenueName, [Location], Capacity, ImageURL)
VALUES ('Kingdom Resort','Pilanesberg','1000','https://www.dcbuilding.com/wp-content/uploads/2017/11/E35C8420-1.jpg'),
('The Kitchen','Edenvale','500','https://th.bing.com/th/id/R.df554ff5913f11de74d0efe226d97927?rik=GWYEU8KsDfLXqQ&pid=ImgRaw&r=0');

INSERT INTO [Event] (EventName, EventDate, [Description], VenueID)
VALUES ('Beauty Spa','2025-05-13','Spa for the family',1),
('Cooking Classes','2025-06-20','Explore the world through your taste buds',2);

INSERT INTO Booking (VenueID, EventID, BookingDate)
VALUES (1, 1, '2025-04-04'),
(2, 2, '2025-03-05');

-- TABLE MANIPULATION SECTION
SELECT * FROM Venue
SELECT * FROM [Event]
SELECT * FROM Booking

SELECT b.BookingID, v.VenueName, v.[Location], e.EventName, e.EventDate, b.BookingDate
FROM Booking b
JOIN Venue v ON b.VenueID = v.VenueID
JOIN [Event] e ON b.EventID = e.EventID
GO

CREATE OR ALTER VIEW BookingView AS
SELECT b.BookingID, b.BookingDate, v.VenueID, v.VenueName, v.[Location] AS VenueLocation, v.Capacity, v.ImageURL AS [Image], e.EventID, e.EventName, e.EventDate, e.[Description] AS Details
FROM Booking b
JOIN Venue v ON b.VenueID = v.VenueID
JOIN [Event] e ON b.EventID = e.EventID
;
GO

SELECT * FROM BookingView
