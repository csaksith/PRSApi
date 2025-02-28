-- Insert Users
INSERT INTO Users (Username, Password, FirstName, LastName, PhoneNumber, Email, Reviewer, Admin)
VALUES
('nailsbycc','ccpassword', 'Celina', 'Saksith','5553907500', 'cc@nailsbycc.com',1,0),
('gelbymishi','misha123','Misha','Scheidt','5139931020','misha@mishamail.com',1,1),
('norinails','nori678','Nori','Smith','5521032800','nori@nmail.com', 0,0),
('acrylicsbyangel','angel666','Angel','Six','5523245656','angel@gmail.com',0,0);

INSERT INTO Vendor (Code, Name, Address, City, State, Zip, PhoneNumber, Email)
VALUES 
('VND001', 'Nail Supply Co.', '123 Beauty St', 'Los Angeles', 'CA', '90001', '5556667777', 'sales@nailsupplyco.com'),
('VND002', 'Glamour Nails Wholesale', '456 Fashion Blvd', 'New York', 'NY', '10001', '5558889999', 'info@glamournails.com'),
('VND003', 'Acrylic Essentials', '789 Salon Way', 'Miami', 'FL', '33101', '5552223333', 'support@acrylicessentials.com');

INSERT INTO Products(VendorID, PartNumber,Name,Price,Unit,PhotoPath)
VALUES
(1, 'GEL001', 'UV Gel Nail Polish - Nude Pink', 12.99, 'Bottle', 'images/gel_nude_pink.jpg'),
(1, 'ACR002', 'Acrylic Powder - Clear', 18.50, 'Jar', 'images/acrylic_clear.jpg'),
(2, 'NAILF003', 'Professional Nail File 100/180 Grit', 2.99, 'Pack of 10', 'images/nail_file.jpg'),
(2, 'NAILB004', 'Electric Nail Buffer', 49.99, 'Unit', 'images/nail_buffer.jpg'),
(3, 'CUTI005', 'Cuticle Oil - Lavender Scent', 8.75, 'Bottle', 'images/cuticle_oil.jpg'),
(3, 'DRILL006', 'Nail Drill Machine', 129.99, 'Unit', 'images/nail_drill.jpg');

INSERT INTO Request(UserID, RequestNumber,Description,Justification,DateNeeded,DeliveryMode,STATUS,TOTAL, SubmittedDate, ReasonForRejection)
VALUES 
(1, 'R2502240001', 'Gel Polish Restock', 'Need more gel polish for upcoming appointments', '2025-02-28', 'Mail', 'NEW', 0.0, GETDATE(), NULL),
(2, 'R2502240002', 'New Acrylic Powders', 'Running low on clear acrylic for nail extensions', '2025-03-01', 'Pickup', 'NEW', 0.0, GETDATE(), NULL),
(3, 'R2502240003', 'Essential Tools Order', 'Need files and buffers for clients', '2025-02-27', 'Mail', 'NEW', 0.0, GETDATE(), NULL);

-- Insert Line Items (Products Ordered in Requests)
INSERT INTO LineItem (RequestID, ProductID, Quantity)
VALUES
(1, 1, 10),  -- Request 1: 10 bottles of UV Gel Polish
(1, 5, 5),   -- Request 1: 5 bottles of Cuticle Oil
(2, 2, 3),   -- Request 2: 3 jars of Acrylic Powder
(2, 6, 1),   -- Request 2: 1 Nail Drill Machine
(3, 3, 20),  -- Request 3: 20 packs of Nail Files
(3, 4, 2);   -- Request 3: 2 Electric Nail Buffers