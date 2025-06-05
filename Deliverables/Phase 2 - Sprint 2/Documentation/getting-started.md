# Library Online Rental System - Development Setup

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Development Setup](#development-setup)
3. [Database Configuration](#database-configuration)
4. [Running the Application](#running-the-application)
5. [Development Workflow](#development-workflow)
6. [Useful Commands](#useful-commands)

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for containerized database)
- [Git](https://git-scm.com/downloads)
- [Rider](https://www.jetbrains.com/rider/) or [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community/) (with ASP.NET and web development workload)
- [MySQL Workbench](https://www.mysql.com/products/workbench/) (optional, for database management)

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

1. **Using Docker (Recommended)**
   - Start MySQL container:
     ```bash
     docker-compose up -d
     ```
   - Wait for the database to initialize (check logs with `docker-compose logs -f`)

2. **Manual Setup**
   - Install MySQL Server 8.0+
   - Create a new database named `library_rental`
   - Update connection string in `appsettings.Development.json`

3. **Apply Migrations**
   ```bash
   dotnet ef database update --project LibraryOnlineRentalSystem
   ```

## Running the Application

1. **Run the API**
   ```bash
   dotnet run --project LibraryOnlineRentalSystem
   ```
   The API will be available at `https://localhost:5001` and `http://localhost:5000`

2. **Access Swagger UI**
   - Open `https://localhost:5001/swagger` in your browser
   - Explore available endpoints and test the API

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
  dotnet ef migrations add YourMigrationName --project LibraryOnlineRentalSystem
  ```

- **Update database**
  ```bash
  dotnet ef database update --project LibraryOnlineRentalSystem
  ```

- **Format code**
  ```bash
  dotnet format
  ```

---

# 1. GIT Setup

## GIT Installation:

Follow this tutorial: https://git-scm.com/downloads

## GIT Configuration

   `git config –global user.name = “Name Surname”
   git config –global user.name = studentnumber@isep.ipp.pt
   git config --global core.editor "nano"`

GUI: https://git-scm.com/downloads/guis


## Configuring SSH Key on Github

Execute the following commands:

   ssh-keygen

After, register the generated key on github:

https://docs.github.com/en/authentication/connecting-to-github-with-ssh/generating-a-newssh-key-and-adding-it-to-the-ssh-agent


# Documentation

## Tools

- **Markdown Editors**
  - [Visual Studio Code](https://code.visualstudio.com/) with [Markdown All in One](https://marketplace.visualstudio.com/items?itemName=yzhang.markdown-all-in-one)
  - [IntelliJ IDEA](https://www.jetbrains.com/idea/)

## API Documentation

- **Swagger UI**
  - Accessible at `/swagger` when running the application
  - Provides interactive API documentation
  - Test endpoints directly from the browser

## Architecture Documentation

- **PlantUML** for diagrams
  - [VS Code Extension](https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml)
  - [Online Editor](https://plantuml.com/)

## Code Snippets

For consistent code screenshots:
- [Carbon](https://carbon.now.sh/)
- [Codeimg.io](https://codeimg.io/)

# Development Tools

## Recommended IDEs

- **Rider**
  - [Download](https://www.jetbrains.com/rider/)
  - Best for .NET development with great C# support
  - Integrated database tools

- **Visual Studio 2022**
  - [Download Community Edition](https://visualstudio.microsoft.com/vs/community/)
  - Install with "ASP.NET and web development" workload
  - Includes SQL Server Data Tools

## Database Tools

- **MySQL Workbench**
  - [Download](https://www.mysql.com/products/workbench/)
  - Visual database design and management
  - SQL development and administration

## API Testing

- **Postman**
  - [Download](https://www.postman.com/downloads/)
  - API testing and documentation
  - Collection runner for automated testing

## Containerization

- **Docker Desktop**
  - [Download](https://www.docker.com/products/docker-desktop/)
  - Required for running the database in a container
  - Useful for consistent development environments
