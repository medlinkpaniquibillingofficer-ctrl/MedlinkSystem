# 🏥 MedlinkDialysisCenter

An internal web-based management system for dialysis center operations, built with **ASP.NET Core MVC (.NET 8)**, **Entity Framework Core**, **SQL Server**, and **Tailwind CSS**.

---

## 📋 Table of Contents

- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Modules](#modules)
- [Default Login Credentials](#default-login-credentials)
- [Database Setup](#database-setup)
- [Development Workflow](#development-workflow)

---

## 🛠 Tech Stack

| Layer        | Technology                          |
|--------------|-------------------------------------|
| Framework    | ASP.NET Core MVC (.NET 8)           |
| ORM          | Entity Framework Core               |
| Database     | SQL Server                          |
| Frontend     | Razor Views + Tailwind CSS          |
| Auth         | ASP.NET Core Identity               |

---

## ✅ Prerequisites

Before running the project, make sure the following are installed on your machine:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or Developer edition)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) *(optional but recommended)*
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# extension
- [Node.js](https://nodejs.org/) *(for Tailwind CSS compilation, if applicable)*
- EF Core CLI tools:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/MedlinkDialysisCenter.git
cd MedlinkDialysisCenter
```

### 2. Configure the Connection String

Open `appsettings.json` and update the connection string to match your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=MedlinkDialysisDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

> If you're using SQL Server with username/password authentication, use:
> ```
> Server=.;Database=MedlinkDialysisDB;User Id=sa;Password=yourpassword;TrustServerCertificate=True;
> ```

### 3. Apply Migrations and Seed the Database

```bash
dotnet ef database update
```

This will create the database and apply all existing migrations. The `DBSeeder.cs` will automatically run on first launch to seed initial data (including the default admin account).

### 4. Run the Application

```bash
dotnet run
```

Or press **F5** in Visual Studio to run with debugging.

The app will be available at:
- `https://localhost:5001` *(HTTPS)*
- `http://localhost:5000` *(HTTP)*

---

## 📁 Project Structure

```
MedlinkDialysisCenter/
├── Controllers/          → MVC Controllers (one per module)
├── Data/
│   └── AppDbContext.cs   → EF Core DbContext
├── Enums/                → Enum types (e.g., InventoryCategory)
├── Helper/
│   └── Extensions/       → Extension methods (e.g., EnumExtensions)
├── Migrations/           → EF Core auto-generated migrations
├── Models/               → Entity/domain models
├── Services/             → Service interfaces + implementations
├── ViewModels/           → ViewModels passed to Razor Views
├── Views/                → Razor Views (organized by module)
├── wwwroot/              → Static files (CSS, JS, images)
├── DBSeeder.cs           → Database seeder (runs on startup)
├── Program.cs            → App entry point + DI configuration
└── appsettings.json      → App configuration
```

**Key conventions:**
- No repository layer — Services use `AppDbContext` directly
- ViewModels (not DTOs) are used for all View bindings
- All services are registered as `AddScoped` in `Program.cs`

---

## 📦 Modules

### 👤 Patient Management
Handles patient records including personal information, medical history, and dialysis session tracking.

- **Controller:** `PatientController.cs`
- **Service:** `IPatientService` / `PatientService`
- **Views:** `Views/Patient/`

### 💊 Inventory
Manages medical supplies and consumables used in dialysis sessions.

- **Controller:** `InventoryController.cs`
- **Service:** `IInventoryItemsService` / `InventoryItemsService`
- **Enum:** `Enums/InventoryCategory.cs`
- **Views:** `Views/Inventory/`

### 🏥 PhConsumption (PhilHealth Session Monitoring)
Tracks PhilHealth session consumption per patient to monitor dialysis benefit usage and ensure compliance.

- **Controller:** `PhConsumptionController.cs`
- **Service:** `IPhConsumptionService` / `PhConsumptionService`
- **Views:** `Views/PhConsumption/`

### 🔐 User / Account Management
Handles user authentication, registration, and role-based access control.

- **Controller:** `AccountController.cs`
- **Views:** `Views/Account/`

---

## 🔑 Default Login Credentials

After running the seeder, use these credentials to log in:

| Field    | Value                  |
|----------|------------------------|
| Email    | `admin@medlink.com`    |
| Password | `Admin@12345`          |

> ⚠️ **Change this password immediately in a production environment.**

---

## 🗄 Database Setup

### Running Migrations

To create a new migration after modifying models:

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

Example:
```bash
dotnet ef migrations add AddPhConsumptionTable
dotnet ef database update
```

### Reverting a Migration

```bash
dotnet ef database update <PreviousMigrationName>
dotnet ef migrations remove
```

---

## 🔧 Development Workflow

### Adding a New Module

Follow this order when scaffolding a new feature:

1. Create the **Model** in `Models/`
2. Create the **ViewModel** in `ViewModels/`
3. Create the **Service interface + implementation** in `Services/`
4. Register the service in `Program.cs`:
   ```csharp
   builder.Services.AddScoped<IYourService, YourService>();
   ```
5. Create the **Controller** in `Controllers/`
6. Create **Razor Views** in `Views/{ModuleName}/`
7. Add the **DbSet** to `AppDbContext.cs` and run a migration

### Naming Conventions

| Item                  | Convention              | Example                   |
|-----------------------|-------------------------|---------------------------|
| Controller            | `{Entity}Controller`    | `PatientController`       |
| Service Interface     | `I{Entity}Service`      | `IPatientService`         |
| Service Impl          | `{Entity}Service`       | `PatientService`          |
| ViewModel             | `{Entity}ViewModel`     | `PatientViewModel`        |
| ViewModel (list)      | `{Entity}ListViewModel` | `PatientListViewModel`    |
| Enum                  | Descriptive             | `InventoryCategory`       |
| Migration             | Descriptive             | `AddPhConsumptionTable`   |

---

## 📝 Notes

- The project uses **Tailwind CSS** for styling with a card-based UI design.
- Modal confirmations are used instead of native browser `alert()`/`confirm()` dialogs.
- All sensitive configuration (connection strings, etc.) should be moved to environment variables or **User Secrets** for production.

---

*Maintained by the Medlink Development Team.*
