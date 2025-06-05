## Index

3. [Ways of working](#ways-of-working)
    - [3.1 Naming conventions](#31-naming-conventions)
    - [3.2 Development patterns](#32-development-patterns)
    - [3.3 Repository branching strategy](#33-repository-branching-strategy)
    - [3.4 Project structure](#34-project-structure)
    - [3.5 Database and ORM](#35-database-and-orm)
    - [3.6 Authentication and Authorization](#36-authentication-and-authorization)
    - [3.7 Code Quality](#37-code-quality)
    - [3.8 Security Guidelines](#38-security-guidelines)

---

## 3. Ways of working

### 3.1 Naming conventions

#### 3.1.1 General

- **PascalCase** for:
  - Class names (e.g., `UserService`, `BookController`)
  - Method names (e.g., `GetUserByIdAsync`)
  - Public properties (e.g., `BookTitle`, `UserEmail`)
  - Namespaces (e.g., `LibraryOnlineRentalSystem.Domain.User`)

- **camelCase** for:
  - Method parameters (e.g., `userId`, `bookTitle`)
  - Private fields (e.g., `_userRepository`)
  - Local variables

- **UPPER_CASE** for constants (e.g., `MAX_RETRY_ATTEMPTS`)

#### 3.1.2 Files and Folders

- Match file names to the primary class they contain (e.g., `UserService.cs` contains `UserService` class)
- Use singular names for entity folders (e.g., `User/`, `Book/`)
- Group related files by feature/domain (e.g., `Domain/User/`, `Controllers/`)
- Configuration files use `appsettings.json` (with `appsettings.Development.json` for development)
- Database migrations are stored in `Migrations/` folder

#### 3.1.3 Code Elements

- Interfaces prefixed with 'I' (e.g., `IUserRepository`)
- Abstract classes have 'Base' or 'Abstract' suffix (e.g., `EntityBase`)
- Generic type parameters prefixed with 'T' (e.g., `TEntity`, `TId`)
- Boolean variables/methods ask a question (e.g., `isValid`, `HasPermission`)
- DTOs end with 'DTO' (e.g., `UserDTO`)
- Value objects are immutable and self-validating

### 3.2 Development patterns

#### 3.2.1 Domain-Driven Design (DDD)

- **Entities**: Rich domain models with identity (e.g., `User`, `Book`)
  ```csharp
  public class User : Entity<UserId>, IAggregateRoot
  {}
  ```

- **Value Objects**: Immutable objects with validation
  ```csharp
  public class Email : IValueObject
  {}
  ```

- **Aggregates**: Transactional boundaries
- **Repositories**: Persistence abstraction
  ```csharp
  public interface IUserRepository
  {}
  ```

- **Domain Services**: Cross-aggregate operations

#### 3.2.2 Clean Architecture

- **Domain Layer**: Core business logic
  - Entities
  - Value Objects
  - Domain Events
  - Domain Services
  - Repository Interfaces

- **Application Layer**: Use cases
  - DTOs
  - Application Services
  - Interfaces for external services

- **Infrastructure Layer**:
  - Entity Framework Core implementations
  - External service implementations
  - Authentication (Keycloak)

- **Presentation Layer**:
  - Controllers
  - API Models
  - Middleware

#### 3.2.3 Exception Handling

- Custom exceptions for business rules
  ```csharp
  public class BusinessRulesException : Exception
  {
      public BusinessRulesException(string message) : base(message) { }
  }
  ```

- Global exception handling middleware
- Structured logging with Serilog
- Standardized error responses

#### 3.2.4 Validation

- **Value Object Validation**: Self-validating value objects
  ```csharp
  // From Email.cs
  public class Email : IValueObject
  {
      private void ValidateEmail(string email)
      {
          var pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
          if (!Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase))
              throw new BusinessRulesException("The email is not valid");
      }
  }
  ```

- **Service Layer Validation**: Business rule validation in services
  ```csharp
  // From UserService.cs
  private void ValidateEmail(string email)
  {
      if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
          throw new BusinessRulesException("Invalid email");
      
      if (email.Contains("\n") || email.Contains("\r"))
          throw new BusinessRulesException("Email contains invalid characters");
  }
  ```

- **Business Rules Exception**: Custom exception for validation failures
  ```csharp
  throw new BusinessRulesException("Email already in use");
  ```

### 3.3 Repository branching strategy

#### 3.3.1 Branch Naming
- `main`: Production code (protected)
- `development`: Integration branch
- `feature/*`: New features
- `bugfix/*`: Bug fixes
- `hotfix/*`: Critical fixes

#### 3.3.2 Workflow
1. Create feature branch from `development`
2. Develop with commits
3. Open PR to `development`
4. Code review required
5. Run all tests
6. Update documentation
7. Merge

### 3.4 Project structure

#### 3.4.1 Key Files
- `Program.cs`: Application startup
- `appsettings.json`: Configuration
- `Dockerfile`: Container configuration
- `docker-compose.yml`: Service definitions
- `init-keycloak-db.sql`: Initial database setup

#### 3.4.2 Directory Structure
```
LibraryOnlineRentalSystem/
│
├── Controllers/               # API Controllers
│   ├── BookController.cs
│   └── UserController.cs
│
├── Domain/                   # Domain models and business logic
│   ├── User/
│   │   ├── User.cs           # User aggregate root
│   │   ├── Email.cs           # Value object
│   │   └── UserService.cs     # Domain service
│   │
│   └── Book/                # Book-related domain models
│
├── Repository/               # Data access layer
│   ├── Common/
│   │   ├── LibraryDbContext.cs
│   │   └── GeneralRepository.cs
│   ├── BookRepository/
│   └── UserRepository/
│
├── appsettings.json          # Configuration
└── Program.cs                # Application entry point
```

### 3.5 Database and ORM

#### 3.5.1 Entity Framework Core
- Code-first approach
- Fluent API for configuration
  ```csharp
  // From LibraryDbContext.cs
  public class LibraryDbContext : DbContext
  {
      public LibraryDbContext(DbContextOptions options) : base(options) { }

      public DbSet<Book> Books { get; set; }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
          modelBuilder.ApplyConfiguration(new ConfigBookEntityType());
      }
  }
  ```

#### 3.5.2 SQL Best Practices
- Use migrations for schema changes
  ```bash
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```
- Indexes for frequently queried columns
- Transactions for multi-operation updates
- Parameterized queries to prevent SQL injection
- Connection strings in configuration

### 3.6 Authentication and Authorization

#### 3.6.1 Authentication
- **JWT-based authentication** with Keycloak integration
  ```csharp
  // From Program.cs
  builder.Services.AddAuthentication(options =>
  {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  })
  .AddJwtBearer(options =>
  {
      options.Authority = builder.Configuration["Keycloak:Authority"];
      options.Audience = builder.Configuration["Keycloak:Audience"];
      options.RequireHttpsMetadata = false; // Set to true in production
  });
  ```
- **Swagger/OpenAPI** with OAuth2 support for testing
- Role-based access control (RBAC) via Keycloak roles
- Secure token validation with issuer and audience checking

#### 3.6.2 Roles
- Admin
- Library Manager
- User
- Guest

### 3.7 Project Dependencies

#### 3.7.1 Core Dependencies
- **.NET 8.0** - Cross-platform development framework
- **Entity Framework Core** (v9.0.5) - ORM for database operations
- **ASP.NET Core** - Web framework

#### 3.7.2 Authentication & Security
- **Microsoft.AspNetCore.Authentication.JwtBearer** (v8.0.1) - JWT authentication middleware
- **Microsoft.AspNetCore.Authentication.OpenIdConnect** (v8.0.0) - OpenID Connect support

#### 3.7.3 Development Tools
- **Swashbuckle.AspNetCore** (v6.6.2) - Swagger/OpenAPI documentation
- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets** (v1.21.0) - Container tooling
- **Migrations** (v1.0.1) - Database migration support

### 3.8 Code Quality

#### 3.8.1 Linting and Formatting
- EditorConfig for consistent style
- dotnet-format for automated formatting
- StyleCop for static analysis

#### 3.8.2 Testing
- xUnit for unit tests
- Moq for mocking
- Test projects parallel to main code
- Integration tests for APIs

#### 3.7.3 Code Reviews
- Required for all PRs
- Check for:
  - Security vulnerabilities
  - Code smells
  - Test coverage
  - Documentation updates

### 3.8 Security Guidelines

#### 3.8.1 Authentication
- JWT with Keycloak
- Secure token storage
- Token refresh mechanism

#### 3.8.2 Authorization
- Role-based access control
- Policy-based authorization
- Resource-based checks

#### 3.8.3 Data Protection
- Encrypt sensitive data
- Use HTTPS everywhere

#### 3.8.4 Dependencies
- Regular updates
- Dependabot for security patches
- Audit for vulnerabilities




