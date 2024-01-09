CREATE DATABASE Market
go
Use Market
go

CREATE TABLE Suppliers (
  SupplierID NVARCHAR(10) PRIMARY KEY,
  SupplierName NVARCHAR(50),
  Address NVARCHAR(255),
  Phone NVARCHAR(10)
);


CREATE TABLE Category ( -- danh mục 
  CategoryID NVARCHAR(10) PRIMARY KEY, 
  CategoryName NVARCHAR(25),
);


CREATE TABLE Products (
  ProductID NVARCHAR(10) PRIMARY KEY,
  ProductName NVARCHAR(50),
  Unit NVARCHAR(20),  -- đơn vị tính
  Price MONEY,
  Amount INT,
  SupplierID NVARCHAR(10),
  CategoryID NVARCHAR(10), -- mã danh mục 
  Img VARBINARY(MAX),

  FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID),
  FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);


CREATE TABLE ShoppingCart( -- giỏ hàng
	ShoppingCartID NVARCHAR(10) PRIMARY KEY,
	ProductID NVARCHAR(10),
	ProductName NVARCHAR(50),
	Unit NVARCHAR(20),  -- đơn vị tính 
	Price MONEY,
	Amount INT,
	Img VARBINARY(MAX),
	Status int,
	Total MONEY,
	FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
);


CREATE TABLE Employees (
  EmployeeID NVARCHAR(10) PRIMARY KEY,
  EmployeeName NVARCHAR(50),
  PhoneNumber NVARCHAR(10),
  Position NVARCHAR(50), -- chức vụ
  Status int
);


CREATE TABLE Orders (
  OrderID NVARCHAR(10) PRIMARY KEY,
  EmployeeID NVARCHAR(10),
  Status NVARCHAR(20),
  OrderDate DATETIME, -- ngày lập đơn
  TotalOrders MONEY,
  CustomerName NVARCHAR(50),
  Address NVARCHAR(255),
  Phone NVARCHAR(10),
  Note NVARCHAR(255)
  FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID),
);


CREATE TABLE OrderDetails (
  OrderID NVARCHAR(10),
  ProductID NVARCHAR(10),
  Price INT,
  Amount INT,
  Sale int,
  Total MONEY,
  CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED 
(
	OrderID ASC,
	ProductID ASC
),
	FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
	FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
);


CREATE TABLE Users (
  UserName NVARCHAR(10) PRIMARY KEY,
  Password NVARCHAR(20),
  EmployeeID NVARCHAR(10),
  Status int
  FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);

ALTER TABLE Products
ALTER COLUMN Detail NVARCHAR(MAX);
ALTER TABLE Products
ALTER COLUMN Img NVARCHAR(50);

ALTER TABLE Orders
ALTER COLUMN Status int null;
ALTER TABLE Category
ADD Status int
ALTER TABLE Products
ADD Status int 
ALTER TABLE Suppliers
ADD Status int
go
--Trigger cập nhật trường TotalOrders mỗi khi thêm sửa xóa bảng OrderDetail
create or alter trigger UpdateTotalOrders
on OrderDetails
after insert, update, delete
as
begin
  update Orders
  set TotalOrders = (select sum(Total) from OrderDetails where OrderDetails.OrderID = Orders.OrderID)
  from Orders
  inner join inserted ON Orders.OrderID = inserted.OrderID
  inner join deleted ON Orders.OrderID = deleted.OrderID;
end
go

--Trigger cập nhật trường Total khi thêm, sửa biết Total = Price*Amount*Sale
create or alter trigger UpdateTotal
on OrderDetails
after insert, update
as
begin
  update OrderDetails
  set Total = OrderDetails.Price*OrderDetails.Amount*(100-OrderDetails.Sale)/100
  from OrderDetails
  inner join inserted ON OrderDetails.OrderID = inserted.OrderID 
				     and OrderDetails.ProductID = inserted.ProductID;
end


