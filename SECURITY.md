# Security Policy

## Security Implementation

This document outlines the security measures implemented in the Library Online Rental System.

### Input Validation

#### User Input
- **Username**: 3-50 chars, alphanumeric + underscores
- **Email**: Valid format, max 100 chars, unique
- **Password**: Min 8 chars, requires upper/lower case, number, special char
- **Phone Number**: Valid Portuguese format (9 digits, specific prefixes)
- **NIF**: Valid Portuguese tax ID with check digit
- **Biography**: Max 500 chars, HTML stripped

#### User Input Validation
- **User Input Validation**: All user inputs are strictly validated on both client and server sides to prevent injection attacks and ensure data integrity.
- **Data Type Validation**: Strong typing is enforced throughout the application with value objects that validate their state on creation.
- **Input Length Restrictions**: 
  - Names: 1-40 characters
  - Usernames: 2-30 characters
  - Email: Standard email format validation
  - Phone Numbers: Exactly 9 digits (Portuguese format)
  - NIF: Exactly 9 digits with format validation
  - Biography: Maximum 150 characters
- **Regular Expression Validation**:
  - Email: Standard email format with additional checks for consecutive dots
  - Phone Numbers: Valid Portuguese phone number formats (mobile, landline)
  - Names: Only letters, spaces, hyphens, and apostrophes
  - Usernames: Must start with a letter, can contain letters, numbers, and underscores (no consecutive underscores)
- **Business Rule Validation**:
  - Unique constraints on email, username, NIF, and phone number
  - Custom validation logic in value objects
  - Business rule exceptions for invalid states

#### Book Input
- **Title**: 
  - 2-100 characters
  - Allows letters, numbers, and standard punctuation
  - Trimmed of leading/trailing whitespace
  - Validated for XSS patterns
  
- **Author**:
  - 2-100 characters
  - Only allows letters, spaces, hyphens, and apostrophes
  - Validated against regex pattern `^[\p{L}\s.'-]+$`
  
- **Category**:
  - 2-50 characters
  - Only allows letters, spaces, and hyphens
  - Validated against regex pattern `^[\p{L}\s-]+$`
  
- **Publisher**:
  - 2-100 characters
  - Allows letters, numbers, spaces, and standard punctuation
  - Validated against regex pattern `^[\p{L}0-9 .,'&()\-]`
  
- **ISBN**:
  - Validates both ISBN-10 and ISBN-13 formats
  - Checksum validation for correct format
  - Case-insensitive comparison
  
- **Description**:
  - Maximum 2000 characters
  - HTML tags are stripped
  - XSS protection against script tags and event handlers
  - Control characters are removed
  - Multiple spaces are collapsed into one
  - Leading/trailing whitespace is trimmed
  
- **Amount of Copies**:
  - Must be a non-negative integer
  - Validated to prevent negative numbers

#### Rental Domain

##### Input Validation
- **Rental Creation**:
  - **Dates**:
    - Start date must be before or equal to end date
    - Maximum rental duration: 30 days
    - Valid ISO 8601 date format required
    - Start date must be in the future
    - End date cannot be more than 30 days from start date
    - Date range cannot overlap with existing rentals for the same book
  
  - **Book Validation**:
    - Book must exist in the system
    - Book must not be marked as deleted
    - Sufficient copies must be available for rental
    - Concurrent rental limit per user (if applicable)
  
  - **User Validation**:
    - Valid email format required
    - User must exist in the system
    - User account must be active
    - User must have necessary permissions to rent books

##### Status Management
- **Valid Status Transitions**:
  - Pending → Active (when approved by library staff)
  - Pending → Cancelled (by user or system)
  - Active → Completed (after return)
  - Active → Cancelled (in special cases)
  
- **Role-Based Status Changes**:
  - **Users** can:
    - Cancel their own pending rentals
    - View their rental history and current rentals
  - **Library Managers** can:
    - Approve pending rentals (set to Active)
    - Mark rentals as Completed
    - Cancel any rental
    - View all rentals in the system
  - **System** can:
    - Auto-cancel expired pending rentals
    - Auto-complete overdue rentals

##### Data Integrity
- **Concurrency Control**:
  - Optimistic concurrency for rental updates
  - Versioning for rental status changes
  - Transaction management for multi-step operations

- **Data Validation**:
  - All IDs (rental, book, user) are validated for existence
  - Input sanitization for all string fields
  - Protection against over-posting attacks

##### Access Control
- **User-Specific Access**:
  - Users can only view their own rentals
  - Users can only cancel their own pending rentals
  - Users cannot modify completed or cancelled rentals

- **Library Manager Access**:
  - Can view all rentals
  - Can modify rental statuses
  - Can manage overdue rentals

- **API Endpoint Security**:
  - All endpoints require authentication
  - Role-based authorization checks
  - Rate limiting on sensitive endpoints
  - Audit logging for all modifications

##### Business Rules
- **Rental Limits**:
  - Maximum concurrent rentals per user
  - Maximum rental duration
  - Blackout periods (e.g., holidays)
  - Reservation expiration (for pending rentals)

- **Notification System**:
  - Rental confirmation
  - Due date reminders
  - Overdue notices
  - Status change notifications

##### Audit and Logging
- **Audit Trails**:
  - All rental status changes
  - User actions (who did what and when)
  - System-generated events

- **Security Logging**:
  - Failed access attempts
  - Unauthorized actions
  - Suspicious patterns

##### Data Protection
- **At Rest**:
  - Sensitive data encryption
  - Secure storage of rental history
  
- **In Transit**:
  - TLS 1.2+ for all communications
  - Secure API endpoints
  
- **Data Retention**:
  - Retention policy for completed rentals
  - Secure deletion of expired rental data

### Data Sanitization

#### 1. HTML/JavaScript Sanitization
- **Implementation**:
  ```csharp
  // Example: Sanitizing user input to prevent XSS
  public string SanitizeHtml(string input)
  {
      if (string.IsNullOrEmpty(input)) return string.Empty;
      
      // Remove all HTML/XML tags
      var sanitized = Regex.Replace(input, "<[^>]*(>|$)", string.Empty);
      
      // Encode special characters
      return WebUtility.HtmlEncode(sanitized);
  }
  ```
- **Key Measures**:
  - Strip all HTML/XML tags using whitelist approach
  - Encode special characters to their HTML entities
  - Use established libraries like `HtmlSanitizer` for complex scenarios
  - Apply context-specific encoding (HTML, JavaScript, URL, CSS)

#### 2. SQL Injection Prevention
- **Implementation**:
  ```csharp
  // Safe: Parameterized query
  var books = await _context.Books
      .Where(b => b.Title.Contains(searchTerm))
      .ToListAsync();
      
  // Safe: Stored procedures with parameters
  var result = await _context.Database
      .SqlQuery<Book>($"EXEC GetBooksByAuthor @authorName", 
          new SqlParameter("@authorName", authorName))
      .ToListAsync();
  ```
- **Key Measures**:
  - Always use Entity Framework Core's parameterized queries
  - For raw SQL, use `FromSqlRaw` or `ExecuteSqlRaw` with parameters
  - Never concatenate user input into SQL queries
  - Use stored procedures with proper parameterization

#### 3. Input Sanitization
- **Implementation**:
  ```csharp
  // Example: Sanitizing user input
  public string SanitizeInput(string input)
  {
      if (string.IsNullOrWhiteSpace(input)) return string.Empty;
      
      // Trim and normalize whitespace
      var sanitized = input.Trim();
      sanitized = Regex.Replace(sanitized, @"\s+", " ");
      
      // Remove control characters
      sanitized = new string(sanitized
          .Where(c => !char.IsControl(c))
          .ToArray());
          
      return sanitized;
  }
  
  // Example: Email validation
  public bool IsValidEmail(string email)
  {
      try {
          var addr = new System.Net.Mail.MailAddress(email);
          return addr.Address == email;
      } catch {
          return false;
      }
  }
  ```
- **Key Measures**:
  - Trim all string inputs
  - Normalize line endings and whitespace
  - Remove control characters (except newlines where appropriate)
  - Validate input against expected patterns (email, phone, etc.)
  - Set maximum length limits on all text fields

#### 4. Output Encoding
- **Implementation**:
  ```csharp
  // In Razor views (automatically encodes output)
  <p>@Model.UserComment</p>
  
  // For JavaScript contexts
  var userData = @JsonSerializer.Serialize(userInput, 
      new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
  ```
- **Key Measures**:
  - Use HTML encoding by default in views
  - Use JavaScript encoding for inline scripts
  - Encode data for the appropriate context (HTML, JavaScript, URL, CSS)
  - Implement Content Security Policy (CSP) headers

#### 5. File Upload Validation
- **Implementation**:
  ```csharp
  public async Task<string> SaveUploadedFile(IFormFile file)
  {
      // Validate file type
      var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
      var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
      if (!allowedExtensions.Contains(extension))
          throw new InvalidOperationException("Invalid file type");

      // Validate file size (e.g., 5MB max)
      if (file.Length > 5 * 1024 * 1024)
          throw new InvalidOperationException("File too large");

      // Scan for malicious content
      if (await IsMaliciousFile(file))
          throw new InvalidOperationException("File contains malicious content");

      // Generate safe filename
      var safeFileName = Path.GetRandomFileName() + extension;
      var filePath = Path.Combine("uploads", safeFileName);
      
      // Save file with limited permissions
      using (var stream = new FileStream(filePath, FileMode.Create))
      {
          await file.CopyToAsync(stream);
      }
      
      return filePath;
  }
  ```
- **Key Measures**:
  - Validate file types using both extension and content inspection
  - Enforce strict size limits
  - Scan files for malware before processing
  - Store files with random, non-guessable names
  - Set appropriate file permissions
  - Serve files with proper Content-Disposition headers

#### 6. Rich Text Sanitization
- **Implementation**:
  ```csharp
  // Using HtmlSanitizer NuGet package
  var sanitizer = new HtmlSanitizer();
  
  // Allow only basic formatting
  sanitizer.AllowedTags.Clear();
  sanitizer.AllowedTags.Add("b");
  sanitizer.AllowedTags.Add("i");
  sanitizer.AllowedTags.Add("p");
  sanitizer.AllowedTags.Add("br");
  
  // Remove all attributes
  sanitizer.AllowedAttributes.Clear();
  
  // Sanitize the input
  string safeHtml = sanitizer.Sanitize(userInput);
  ```
- **Key Measures**:
  - Use whitelist approach for allowed HTML tags
  - Strip all attributes unless explicitly allowed
  - Remove potentially dangerous content (scripts, iframes, etc.)
  - Consider using Markdown instead of HTML for user content

#### 7. Log Sanitization
- **Implementation**:
  ```csharp
  public void LogSensitiveData(string userId, string action, string data)
  {
      // Redact sensitive information
      var redactedData = Regex.Replace(data, 
          @"\b(\d{4})(\d{4,8})(\d{4})\b", // Credit card pattern
          m => $"{m.Groups[1].Value}******{m.Groups[3].Value}");
          
      _logger.LogInformation($"User {userId} performed {action} with data: {redactedData}");
  }
  ```
- **Key Measures**:
  - Never log sensitive data (passwords, tokens, PII)
  - Redact or hash sensitive information in logs
  - Use structured logging with proper log levels
  - Implement log rotation and retention policies

### Data Protection

- **At Rest**: AES-256 encryption
- **In Transit**: TLS 1.2+
- **Secrets**: Stored in Azure Key Vault
- **Database**: Parameterized queries, principle of least privilege

### Authentication & Authorization

- **RBAC**:
  - Admin: Full access, cant rental or add books
  - LibraryManager: Book/rental management
  - User: Personal access only
- **JWT**: 1h access tokens, 7d refresh tokens
- **Keycloak**: Centralized identity management

### Monitoring & Logging
- Azure Application Insights integration
- Security event logging
- Rate limiting on auth endpoints
- Structured logging with correlation IDs

## Recent Security Updates (2024-03-26)

### Authentication
- Enhanced JWT validation
- Secure token storage with HttpOnly cookies
- Token refresh mechanism

### Authorization
- Strict role validation in all controllers
- Resource ownership verification
- Admin/LibraryManager access restrictions

### Input Validation
- Comprehensive model validation
- Custom validation attributes
- Consistent error responses

## Security Best Practices
1. Use strong, unique passwords
2. Enable 2FA
3. Keep system updated
4. Regular security testing
5. Follow principle of least privilege
6. No secret keys exposed in code
7. Password not stored in database, handled by keycloak
