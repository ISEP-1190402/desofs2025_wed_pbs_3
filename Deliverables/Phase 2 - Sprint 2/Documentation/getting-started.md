# Library Online Rental System - Development

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Development Setup](#development-setup)
3. [Database Configuration](#database-configuration)
4. [Running the Application](#running-the-application)
5. [Development Workflow](#development-workflow)
6. [Useful Commands](#useful-commands)

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- MySQL Server 8.0+
- [Git](https://git-scm.com/downloads)
- [Rider](https://www.jetbrains.com/rider/) or [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community/) (with ASP.NET and web development workload)
- [MySQL Workbench](https://www.mysql.com/products/workbench/) (recommended for database management)
- Keycloak 26.2.5 (for authentication)

## Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/ISEP-1190402/desofs2025_wed_pbs_3.git
   cd desofs2025_wed_pbs_3
   ```

2. **Switch to development branch**
   ```bash
   git checkout development
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

## Database Configuration

1. **Database Setup**
   - Install MySQL Server 8.0+
   - Create two databases:
     ```sql
     CREATE DATABASE librarydb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
     CREATE DATABASE keycloak CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
     ```
   - Update connection strings in `appsettings.Development.json`
   - Ensure the database user has proper permissions on both databases

2. **Apply Migrations**
   ```bash
   dotnet ef database update --project LibraryOnlineRentalSystem
   ```

3. **Keycloak Setup**
   - Install Keycloak 26.2.5
   - Configure Keycloak to use the MySQL database
   - Set up the realm and client as specified in the Keycloak documentation
   - Configure the application to use Keycloak for authentication

## Running the Application

1. **Start Keycloak**
   ```bash
   cd "C:\Program Files\keycloak-26.2.5\bin"
   kc.bat start-dev 
   ```

2. **Run the API**
   ```bash
   dotnet run --project LibraryOnlineRentalSystem
   ```

3. **Access Swagger UI**
   - Open `http://51.105.240.143/index.html` in your browser
   - Explore available endpoints and test the API

4. **Access Keycloak Admin Console**
   - Open `http://localhost:8080/admin/` in your browser
   - Log in with admin credentials

## Development Workflow

1. **Branching Strategy**
   - `main` - Production code (protected)
   - `development` - Integration branch for features
   - `feature/*` - New features
   - `bugfix/*` - Bug fixes
   - `hotfix/*` - Critical production fixes

2. **Pull Requests**
   - Create PRs from feature/bugfix branches to `development`
   - At least one approval required
   - All tests must pass
   - Update documentation if needed

## Useful Commands

- **Run tests**
  ```bash
  dotnet test
  ```

- **Add a new migration**
  ```bash
  dotnet ef migrations add MigrationName
  ```

- **Update database**
  ```bash
  dotnet ef database update
  ```

- **Format code**
  ```bash
  dotnet format
  ```

- **Remove last migration**
  ```bash
  dotnet ef migrations remove
  ```

- **Start Keycloak in development mode**
  ```bash
  cd "C:\Program Files\keycloak-26.2.5\bin"
  kc.bat start-dev --http-port=8080
  ```

