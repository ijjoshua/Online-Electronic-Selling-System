# Online Electronic Selling System (OESS)

A .NET 10 Windows Forms desktop application using an MVC-oriented structure and MySQL persistence.

## Implemented MVP

- Customer registration and role-aware login (Customer, Admin, Seller, Sales Officer, Support Agent, Delivery Agent)
- Product search and catalogue display
- Product and inventory maintenance for staff
- Stock validation and transactional stock reduction at checkout
- Persistent customer cart
- Orders and payment method/status recording
- Staff dashboard report (orders, revenue, pending orders, low stock, customers)
- Advanced staff modules for customer status, suppliers, coupons, support tickets, returns, warranty claims, and audit logs
- Parameterized MySQL queries and salted PBKDF2 password verification

External payment gateways, email/SMS, shipping integrations, returns, warranty, reviews, and advanced reports are planned extensions of the SRS and are not simulated as completed integrations in this MVP.

## Requirements

- Windows with .NET 10 SDK/runtime
- MySQL Server 8 or compatible MySQL installation
- Visual Studio 2026 or `dotnet` CLI

## Database setup

1. Start MySQL and create the database/user (or use an existing account):

    ```sql
    CREATE DATABASE oess CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
    ```

2. For the complete SRS database, import `database/oess_schema.sql` in phpMyAdmin:
   - Open `http://localhost/phpmyadmin/`.
   - Select **Import** from the server or database page.
   - Choose `database/oess_schema.sql`.
   - Leave the format as SQL and click **Import**.
   - The script creates or selects the `oess` database and creates the tables.

   Alternatively, open the phpMyAdmin **SQL** tab and paste/run the script contents.

3. Verify the import by selecting the `oess` database and running:

```sql
USE oess;
SHOW TABLES;
SELECT COUNT(*) AS products FROM products;
SELECT name FROM roles ORDER BY name;
```

4. Copy the example configuration and set the real values:

    ```powershell
    Copy-Item .\OnlineElectronicSellingSystem\appsettings.example.json .\OnlineElectronicSellingSystem\appsettings.json
    ```

   Edit `OnlineElectronicSellingSystem/appsettings.json` and set `Server`, `Port`, `Database`, `User ID`, and `Password`. The local `appsettings.json` is ignored by Git. Alternatively set the `OESS_CONNECTION_STRING` environment variable.

5. Run the application. Startup creates any missing MVP tables and sample data automatically.
6. Initial development login: `admin@oess.local` / `admin`. Change or remove this seed account before deployment.

The schema file creates the complete data model. The current WinForms GUI is implemented as an MVP; advanced tables such as warranties, returns, suppliers, support, notifications, and audit logs are ready for later controllers and screens.

Do not commit production credentials. Use the ignored local configuration file or `OESS_CONNECTION_STRING` in the deployment environment.

## Run

```powershell
dotnet restore
dotnet build
dotnet run --project .\OnlineElectronicSellingSystem\OnlineElectronicSellingSystem.csproj
```

## MVC structure

- `Models` contains domain models and view data.
- `Repositories` contains parameterized MySQL persistence and transactions.
- `Controllers` contains authentication, product, sales, and reporting workflows plus role checks.
- `Form1.cs` is the WinForms presentation layer and binds UI actions to controllers.
- `Data` contains connection, schema initialization, and password hashing.

## Pre-push checklist

1. Confirm `dotnet build` succeeds.
2. Confirm MySQL is running and the local `appsettings.json` is configured, or set `OESS_CONNECTION_STRING`.
3. Launch the application and verify admin login, customer registration, product search, add-to-cart, checkout, order history, and staff reports.
4. Confirm `git status --ignored` does not show production credentials being staged.
5. Review the staged files before committing.

## Publish to GitHub

Create an empty repository on GitHub, then run these commands from the repository root:

```powershell
git init
git add .
git status
git commit -m "Implement OESS WinForms MVC MVP"
git branch -M main
git remote add origin https://github.com/<your-account>/<your-repository>.git
git push -u origin main
```

Replace the remote URL with the repository URL created for the assignment. Never commit passwords or production connection strings.
