## Index

3. [Ways of working](#ways-of-working)
    - [3.1 Naming conventions](#31-naming-conventions)
    - [3.2 Development patterns](#32-development-patterns)
    - [3.3 Repository branching strategy](#33-repository-branching-strategy)
    - [3.4 Project structure](#34-project-structure)
    - [3.5 Database and ORM](#35-database-and-orm)
    - [3.6 Authentication and Authorization](#36-authentication-and-authorization)
    - [3.7 Project Dependencies](#37-project-dependencies)
    - [3.8 Presentation Layer Components](#38-presentation-layer-components)
    - [3.9 Code Quality](#39-code-quality)
    - [3.10 Security Guidelines](#310-security-guidelines)
    - [3.11 Deployment](#311-deployment)

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
  - Local variables (e.g., `response` in `BookController.GetAllBooks`, `currentUserId` in `UserController`, `guid` in `UserIdTest`)

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

- **Value Objects**: Immutable objects with validation (e.g., `Email`, `PhoneNumber`, `UserName`)
  ```csharp
  // Email value object with comprehensive validation
  public class Email : ICloneable, IValueObject
  {
      public string EmailAddress { get; }

      public Email() { }
      
      // To see full implementation, see Email.cs
  }
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
  - Entities (e.g., `User`, `Book`)
  - Value Objects (e.g., `Email`, `PhoneNumber`)
  - Domain Services (e.g., `UserService`)
  - Repository Interfaces (e.g., `IUserRepository`)
  - Domain-specific exceptions (e.g., `BusinessRulesException`)

- **Application Layer**:
  - DTOs (e.g., `UserDTO`, `NewUserDTO`)
  - Application Services
  - Interfaces for external services
  - Request/Response models
  - Validation logic

- **Infrastructure Layer**:
  - Entity Framework Core implementations
  - Repository implementations
  - Database context and configurations
  - Authentication services (Keycloak integration)
<!--  - External service clients-->

- **Presentation Layer**:
  - **Controllers**: Handle HTTP requests and responses
    - `UserController`: User management endpoints
    - `BookController`: Book management endpoints
    - `AuthController`: Authentication endpoints
  - **Middleware**: Request/response pipeline components
  - **API Documentation**: Swagger/OpenAPI integration
  - **DTOs**: Data transfer objects for API contracts
  - **Validation**: Request model validation

#### 3.2.3 Exception Handling

- Custom exceptions for business rules
  ```csharp
  public class BusinessRulesException : Exception
  {
      public BusinessRulesException(string message) : base(message) { }
  }
  ```

- Global exception handling
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
          if (email.Contains(".."))
              throw new BusinessRulesException("The email is not valid");
      }
  }
  ```

- **Service Layer Validation**: Business rule validation in services
  ```csharp
  // From UserService.cs
  public async Task CreateUserAsync(NewUserDTO req)
  {
      if (await _userRepository.GetByEmailAsync(req.Email) != null)
          throw new BusinessRulesException("Email already in use");

      if (await _userRepository.GetByUsernameAsync(req.UserName) != null)
          throw new BusinessRulesException("Username already in use");
      
      // Additional validation logic...
  }
  ```

- **DTO Validation**: Input validation using data annotations
  ```csharp
  public class NewUserDTO
  {
      [Required]
      public string Name { get; set; }
      
      [Required]
      [EmailAddress]
      public string Email { get; set; }
      
      [Required]
      [StringLength(50, MinimumLength = 3)]
      public string UserName { get; set; }
      
      [Required]
      [StringLength(100, MinimumLength = 6)]
      public string Password { get; set; }
      
      // Other properties...
  }
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
- `appsettings.Development.json`: Development-specific settings
- `init-keycloak-db.sql`: Initial database setup
- `init-keycloak.sh`: Keycloak initialization script

#### 3.4.2 Directory Structure
```
LibraryOnlineRentalSystem/
│
├── Controllers/               # API Controllers
│   ├── AuthController.cs      # Authentication endpoints
│   ├── BookController.cs      # Book management endpoints
│   ├── RoleController.cs      # Role management endpoints
│   └── UserController.cs      # User management endpoints
│
├── Domain/                   # Domain layer
│   ├── Book/                 # Book domain
│   │   ├── Book.cs           # Book entity
│   │   ├── IBookRepository.cs # Book repository interface
│   │   └── BookService.cs    # Book domain service
│   │
│   ├── Common/             # Shared domain components
│   │   ├── IRepository.cs    # Generic repository interface
│   │   ├── IUnitOfWork.cs    # Unit of work pattern
│   │   └── ValueObject.cs    # Base class for value objects
│   │
│   ├── Role/               # Role domain
│   │   └── Role.cs           # Role entity
│   │
│   └── User/               # User domain
│       ├── User.cs           # User aggregate root
│       ├── Email.cs          # Value object
│       ├── UserService.cs    # Domain service
│       ├── IUserRepository.cs # User repository interface
│       └── DTOs/             # User-related DTOs
│           ├── UserDTO.cs
│           └── NewUserDTO.cs
│
├── Repository/               # Data access layer
│   ├── Common/
│   │   ├── LibraryDbContext.cs  # DbContext configuration
│   │   └── BaseRepository.cs  # Generic repository implementation
│   ├── BookRepository/       # Book repository implementation
│   └── UserRepository/       # User repository implementation
│
├── Files/                   # File storage
│   └── ...                   # Uploaded files and documents
│
├── Migrations/             # Database migrations
│   └── ...                   # EF Core migration files
│
├── Properties/              # Assembly metadata and launch settings
├── Utils/                   # Utility classes and helpers
├── appsettings.json         # Application configuration
├── appsettings.Development.json # Development settings
├── init-keycloak-db.sql     # Keycloak database setup
├── init-keycloak.sh         # Keycloak initialization script
├── LibraryOnlineRentalSystem.csproj # Project file
├── LibraryOnlineRentalSystem.sln    # Solution file
└── Program.cs               # Application entry point
```

### 3.5 Database and ORM

#### 3.5.1 Entity Framework Core
- **Code-first** approach with MySQL database
- **Fluent API** for configuration
  ```csharp
  // From LibraryDbContext.cs
  public class LibraryDbContext : DbContext
  {
      public LibraryDbContext(DbContextOptions options) : base(options) { }

      public DbSet<Book> Books { get; set; }
      public DbSet<User> Users { get; set; }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
          modelBuilder.ApplyConfiguration(new ConfigBookEntityType());
          modelBuilder.ApplyConfiguration(new ConfigUserEntityType());
      }
  }
  ```
- **MySQL** database provider with Pomelo
<!-- - **Value conversion** for custom types -->

#### 3.5.2 SQL Best Practices
- Use migrations for schema changes
  ```bash
  dotnet ef migrations add LibraryOnlineRentalSystem
  dotnet ef database update
  ```
- Parameterized queries to prevent SQL injection
<!-- - Indexes for frequently queried columns
- Transactions for multi-operation updates
- Connection strings in configuration -->

### 3.6 Authentication and Authorization

#### 3.6.1 Authentication
- **JWT-based authentication** with Keycloak integration
  ```csharp
  // From Startup.cs
  services.AddAuthentication(options =>
  {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  })
  .AddJwtBearer(options =>
  {
      options.Authority = Configuration["Keycloak:Authority"];
      options.Audience = Configuration["Keycloak:Audience"];
      options.RequireHttpsMetadata = false; // Set to true in production
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true
      };
  });
  ```
- **Swagger/OpenAPI** with OAuth2 support for testing
- **Role-based access control (RBAC)** via Keycloak roles
- **Secure token validation** with issuer and audience checking
- **Password hashing** using BCrypt
  ```csharp
  // Password hashing in PasswordService
  var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
  ```

#### 3.6.2 Roles and Authorization
- **Admin**: Full system access
  ```csharp
  [Authorize]
  [Authorize(Roles = "Admin")]
  ```
- **LibraryManager**: Manage books and users
  ```csharp
  [Authorize]
  [Authorize(Roles = "LibraryManager")]
  ```
- **User**: Regular authenticated users
  ```csharp
  [Authorize]
  [Authorize(Roles = "User")]
  ```
- **Guest**: Unauthenticated access (limited endpoints)

### 3.7 Project Dependencies

#### 3.7.1 Core Dependencies
- **.NET 8.0** - Cross-platform development framework
- **Entity Framework Core** (v8.0.13) - ORM for database operations
- **Pomelo.EntityFrameworkCore.MySql** (v8.0.3) - MySQL database provider
- **ASP.NET Core** - Web framework
- **Newtonsoft.Json** (v13.0.3) - JSON serialization

#### 3.7.2 Authentication & Security
- **Microsoft.AspNetCore.Authentication.JwtBearer** (v8.0.0) - JWT authentication middleware
- **Microsoft.AspNetCore.Authentication.OpenIdConnect** (v8.0.0) - OpenID Connect support
- **BCrypt.Net-Next** (v4.0.3) - Password hashing
- **Microsoft.Extensions.Configuration** (v10.0.0-preview.4) - Configuration management

#### 3.7.3 Development Tools
- **Swashbuckle.AspNetCore** (v6.5.0) - Swagger/OpenAPI documentation
- **NSwag.AspNetCore** (v14.0.0) - API documentation
- **Microsoft.VisualStudio.Azure.Containers.Tools.Targets** (v1.21.0) - Container tooling
- **Migrations** (v1.0.1) - Database migration support
- **FluentValidation.AspNetCore** (v11.3.0) - Model validation

### 3.8 Presentation Layer Components

#### 3.8.1 API Controllers

##### User Controller
- **Base Route**: `/api/User`
- **Endpoints**:
  ```csharp
  // Register new user (Public)
  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] NewUserDTO request)
  
  // Get user by ID (Admin/LibraryManager)
  [HttpGet("{id}")]
  [Authorize(Roles = "Admin,LibraryManager")]
  public async Task<IActionResult> GetUser(Guid id)
  
  // Get all users (Admin/LibraryManager)
  [HttpGet]
  [Authorize(Roles = "Admin,LibraryManager")]
  public async Task<IActionResult> GetAllUsers()
  
  // Update user (Admin or self)
  [HttpPut("{id}")]
  [Authorize]
  public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
  ```

#### 3.8.2 Data Transfer Objects (DTOs)

##### NewUserDTO
```csharp
public class NewUserDTO
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string UserName { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }
    
    public string PhoneNumber { get; set; }
    public string Nif { get; set; }
    public string Biography { get; set; }
}
```

##### UpdateUserRequest
```csharp
public class UpdateUserRequest
{
    public string? PhoneNumber { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Biography { get; set; }
}
```

#### 3.8.3 API Documentation
- **Swagger/OpenAPI** integration
- **OAuth2** authentication for testing
<!-- - **XML documentation** for API endpoints
- **Example requests/responses**-->

### 3.9 Code Quality

#### 3.9.1 Linting and Formatting
<!-- - **EditorConfig** for consistent code style
- **StyleCop** for static code analysis
- **XML documentation** for public APIs -->
- **dotnet-format** for automated code formatting


#### 3.9.2 Testing
- **xUnit** for unit tests
<!-- - **Moq** for mocking dependencies -->
- **Test projects** parallel to main code
- **Integration tests** for API endpoints
- **Test coverage** tracking

#### 3.9.3 Code Reviews
- **Pull request** workflow
- **Code ownership** and responsibility
- **Automated checks** before merge
- **Documentation** updates required
- **Check for**:
  - Security vulnerabilities
  - Code smells
  - Test coverage
  - Documentation updates

### 3.10 Security Guidelines

#### 3.10.1 Authentication & Authorization
- **Keycloak Integration**:
  - Centralized authentication using Keycloak
  - OAuth 2.0 and OpenID Connect protocols
  - Role-based access control (RBAC)
  - JWT token validation for all API endpoints
  - Token refresh mechanism implementation
  - Secure token storage
  - Short-lived access tokens with secure refresh tokens
  - Token revocation on logout
  - Multi-factor authentication (MFA) support
  - Account lockout after failed attempts

#### 3.10.2 Input Validation
- **Multi-layer Validation**:
  - Client-side validation for UX
  - Server-side validation in controllers using Data Annotations
  - Domain-level validation in value objects
  - Database constraints as final safety net
- **Validation Rules**:
  - Regex-based validation for all inputs
  - Character whitelisting for specific fields
  - Length validation with min/max constraints
  - Format validation for emails, phones, NIFs
  - NoSQL injection prevention
  - File upload validation (type, size, content)
  - Input sanitization before processing

#### 3.10.3 Data Protection
- **Encryption**:
  - Data at rest encryption
  - TLS 1.2+ for data in transit
  - Secure key management
  - Certificate management
- **Database Security**:
  - Principle of least privilege for DB users
  - Encrypted database connections
  - Sensitive data encryption
  - Regular security patches

#### 3.10.4 API Security
- **HTTPS**: Enforced in all environments
- **CORS**: Strict origin validation
- **Headers**: Security headers (HSTS, CSP, XSS-Protection)
- **Rate Limiting**: Protection against brute force attacks
- **Request Validation**: All input sanitized and validated
- **API Versioning**: Clear versioning strategy
- **Request/Response Logging**: Sensitive data redaction
- **API Gateway**: WAF integration

#### 3.10.5 Security Headers
- **Content-Security-Policy**: `default-src 'self'`
- **X-Content-Type-Options**: `nosniff`
- **X-Frame-Options**: `DENY`
- **Strict-Transport-Security**: `max-age=31536000; includeSubDomains`
- **X-XSS-Protection**: `1; mode=block`
- **Referrer-Policy**: `strict-origin-when-cross-origin`
- **Permissions-Policy**: `camera=(), microphone=(), geolocation=()`
- **Cache-Control**: `no-store, no-cache, must-revalidate`

#### 3.10.6 Dependency Management
- **Automated Scans**:
  - Dependabot for dependency updates
  - OWASP Dependency-Check
  - License compliance checks
  - Regular security audits
- **Patching**:
  - Critical patches within 24 hours
  - Regular dependency updates
  - Vulnerability monitoring

#### 3.10.7 Security Testing
- **SAST**: Static Application Security Testing
- **DAST**: Dynamic Application Security Testing
- **IAST**: Interactive Application Security Testing
- **SCA**: Software Composition Analysis
- **Penetration Testing**: Regular security assessments
- **Red Team Exercises**: Simulated attacks

#### 3.10.8 Incident Response
- **Monitoring**:
  - Real-time security event monitoring
  - Automated alerting for suspicious activities
  - Log analysis for security events
- **Response Plan**:
  - Documented incident response procedures
  - Designated security contacts
  - Post-incident review process
  - Communication plan for breaches

#### 3.10.9 Email Security and Warning System
- **Development Email Handling**:
  - All emails in development are redirected to a single development email address
  - Real user emails are never used in development
  - Comprehensive logging of all email sending attempts and failures
- **Configuration**:
  - SMTP settings configured in `appsettings.Development.json`
  - Sensitive credentials stored in environment variables or user secrets
  - Development email redirection is automatically disabled in production
- **Security Measures**:
  - No hardcoded email credentials in source control
  - Input validation for all email addresses
  - Rate limiting to prevent email abuse
  - Secure storage of SMTP credentials

#### 3.10.10 Secure Development Lifecycle
- **Threat Modeling**: During design phase
- **Secure Code Reviews**: Mandatory for all changes
- **Security Testing**: Integrated in CI/CD
- **Deployment Security**: Infrastructure as Code scanning
- **Compliance Checks**: Regular security audits

#### 3.10.10 Security Awareness
- **Training**: Regular security training for developers
- **Documentation**: Security guidelines and best practices
- **Code Examples**: Secure coding patterns
- **Security Champions**: Designated team members

### 3.11 Deployment

#### 3.11.1 Environments
- **Development** - Local development environment on Windows Server 2022 Virtual Machine
  - Local MySQL Server 8.0+
  - Local Keycloak 26.2.5 instance
  - Accessible at `http://localhost:8080`
- **Production** - Azure VM with IIS 10.0
  - MySQL Server 8.0+
  - Keycloak 26.2.5
  - Windows Server 2022

#### 3.11.2 CI/CD Pipeline
- **GitHub Actions** workflow for automation
- **IIS Deployment** to Azure VM using self-hosted runner
- **Zero-downtime** deployment strategy

**Security Pipelines**:
- **SAST (Static Application Security Testing)**: Automated code analysis with Snyk to identify security vulnerabilities in the source code.
- **DAST (Dynamic Application Security Testing)**: Automated security testing of the running application using OWASP ZAP to find runtime vulnerabilities.
- **IAST (Interactive Application Security Testing)**: Combines Snyk and ZAP for interactive security testing during runtime.
- **CSA (Combined Security Analysis)**: Comprehensive pipeline integrating SonarCloud, Snyk, and ZAP for complete security coverage.
- **Deployment Pipeline**: Automated build and deployment to IIS with artifact management and environment configuration.

Detailed documentation for each pipeline is available in the `Documentation/Pipeline/` directory.

#### 3.11.3 Deployment Process

Detailed deployment documentation is available in [Deploy Pipeline Documentation](./Pipeline/Deploy%20Pipeline/Deploy-Pipeline.md).

1. **Build Stage** (GitHub-hosted runner):
   - .NET 8.x SDK setup
   - Solution build in Release configuration
   - Application publish to working directory
   - Artifact upload

2. **Deploy Stage** (Self-hosted runner):
   - Artifact download
   - Clean target directory (`C:/inetpub/wwwroot/LibraryOnlineRentalSystem/`)
   - File copy to IIS directory
   - Application pool recycle

For infrastructure setup and configuration details, including IIS and GitHub runner setup, refer to the [Infrastructure Documentation](./Infrastructure/Infrastructure.md).

#### 3.11.4 Access Points
- **Production API**: http://51.105.240.143/
- **Swagger UI**: http://51.105.240.143/index.html
- **Keycloak Admin**: http://localhost:8080/admin/

#### 3.11.5 Rollback Procedure
1. Revert the last commit in `main` branch
2. Push changes to trigger a new deployment
3. Or manually deploy a previous version from GitHub Actions artifacts

#### 3.11.6 Database Migrations
- Run migrations manually after deployment if needed:
  ```bash
  dotnet ef database update --project LibraryOnlineRentalSystem
  ```
- Ensure database user has proper permissions on both `librarydb` and `keycloak` databases
