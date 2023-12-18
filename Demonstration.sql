--TABLES

select * from Allergen;
select * from Category;
select * from Company;
select * from History;
select * from [Order];
select * from Product;
select * from [User];

--FUNCTIONS

select [dbo].CheckEmailFunc('plutoe98@gmail.com');
declare @id int
exec Reg 'new', '1', 'addr', 'plutoe98@gmail.com', 'pass', @id
delete [User] where Email = 'plutoe98@gmail.com'
select [dbo].CountUsers();
select [dbo].CompanyAveragePrice(1);
select * from [dbo].AveragePrices();
select * from [dbo].GetRCP();
select * from [dbo].GetOrdersFunc();

--PROCEDURES

--���� � �������� �������
declare @isadmin bit, @id int, @key int			
exec Auth 'admin', '1', @isadmin, @id, @key

--���� � ������ �������
declare @isadmin bit, @id int, @key int			
exec Auth 'admin', 'admin', @isadmin, @id, @key

--�������� ������
exec ChangeStatus 1002, '���'

--�������� ID
exec ChangeStatus 0, '� ����'

--�������� ID ��������
exec CreateCategory 'test', 90121

--�������� ID
exec DeleteCategory 01923

--������������ �����
declare @id int
exec Reg 'admin', '123', 'admin', 'admin', 'admin', @id

--JSON

exec UserToJson
CREATE TABLE [dbo].[UserImport](
	[IDUser] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[PhoneNo] [nvarchar](50) NOT NULL,
	[Address] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Password] [varbinary](500) NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[AdminKey] [int]
)
SET IDENTITY_INSERT UserImport ON
select * from UserImport
exec UserFromJson
drop table UserImport
