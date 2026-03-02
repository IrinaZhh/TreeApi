This project is an ASP.NET Core 8 REST API for managing independent tree structures and tracking exceptions through a custom exception journal.
The application uses MS SQL Server with Entity Framework Core (Code-First approach).

⚠️ Before running the application, you must create a database in MS SQL Server and update the connection string in appsettings.json.

1. Run the following SQL script in SQL Server Management Studio (SSMS):

CREATE DATABASE TreeApiDB;
GO

2. Make sure your appsettings.json contains:
3. 
ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TreeApiDB;Trusted_Connection=True;TrustServerCertificate=True;"
}

3.After configuring the database, apply migrations using:

dotnet ef database update
