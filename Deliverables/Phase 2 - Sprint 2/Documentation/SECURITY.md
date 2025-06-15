# Security Implementation Documentation

## Table of Contents
1. [Input Validation](#input-validation)
2. [Data Sanitization](#data-sanitization)
3. [Authentication & Authorization](#authentication--authorization)
4. [Data Protection](#data-protection)
5. [API Security](#api-security)
6. [Dependency Security](#dependency-security)
7. [Logging & Monitoring](#logging--monitoring)

## Input Validation

### Book Domain

#### Book Name
- **Length**: 2-200 characters
- **Characters**: Letters, numbers, spaces, and basic punctuation (.,:;!?()&'-)
- **Case**: Case-insensitive comparison
- **Whitespace**: Automatic trimming
- **Security**: Basic XSS prevention through allowed character restrictions
- **Validation**: 
  - Regex pattern: /^[a-zA-Z0-9 .,;!?()&'-]+$/
  - Minimum of 2 non-whitespace characters required

#### ISBN
- **Formats**: ISBN-10 and ISBN-13
- **Input**: Accepts both hyphenated and non-hyphenated formats
- **Case**: Case-insensitive comparison for all characters
- **No checksum validation** (as per requirements)
- **Length**: 10 or 13 digits (after removing hyphens)
- **Uniqueness**: Enforced at application level
- **Validation**:
  - Removes hyphens and spaces
  - Converts to uppercase
  - Validates length and character set
  - Regex pattern: /^[0-9]{10}$|^[0-9]{13}$/

#### Category
- **Length**: 2-50 characters
- **Characters**: Letters, spaces, and hyphens only (no numbers)
- **Case**: Case-insensitive comparison
- **Flexibility**: No predefined list, allowing library manager's discretion
- **Validation**: 
  - Regex pattern: /^[a-zA-Z\s-]+$/
  - Minimum of 2 non-whitespace characters required

#### Description
- **Length**: 5-2000 characters (reduced minimum from 10 to 5)
- **Security**: XSS protection through input sanitization
- **Content**: Allows standard text with basic punctuation
- **HTML**: Strips HTML tags to prevent XSS
- **Validation**: 
  - HTML tags are removed
  - Maximum length enforced
  - Basic markdown formatting allowed (bold, italic, links)

#### Author
- **Length**: 2-100 characters
- **Characters**: Letters, spaces, hyphens (-), apostrophes ('), and periods (.) only (no numbers)
- **Whitespace**: Automatic trimming
- **Comparison**: Case-insensitive
- **Validation**: 
  - Regex pattern: /^[a-zA-Z\s'-\.]+$/
  - Minimum of 2 non-whitespace characters required

#### Publisher
- **Length**: 2-100 characters
- **Characters**: Letters, numbers, spaces, and basic punctuation
- **Whitespace**: Automatic trimming
- **Comparison**: Case-insensitive
- **Validation**: 
  - Regex pattern: /^[a-zA-Z0-9\s.,;!?()&'-]+$/
  - Minimum of 2 non-whitespace characters required

#### AmountOfCopies
- **Range**: 0 to 1500 (inclusive)
- **Type**: Integer values only
- **Null Safety**: Non-nullable
- **Validation**: 
  - Must be integer
  - Range validation
  - No decimal points allowed

### User Domain

#### Email
- **Format**: Standard email validation
- **Case**: Case-insensitive
- **Length**: Reasonable length limits enforced
- **Validation**: 
  - RFC 5322 compliant regex
  - Maximum length: 254 characters
  - Must contain @ and domain part
  - Domain must have valid TLD

#### NIF (Portuguese Tax ID)
- **Format**: 9 digits
- **Validation**: 
  - Check digit validation according to Portuguese NIF rules
  - Must be exactly 9 digits
  - No special characters allowed
  - Regex pattern: /^[0-9]{9}$/

#### PhoneNumber
- **Format**: 9 digits (Portuguese format)
- **Optional**: Country code support
- **Separators**: Allows spaces, hyphens, dots
- **Validation**: 
  - Must be valid Portuguese phone number
  - Supports +351 prefix
  - Regex pattern: /^(\+351\s?)?\d{9}$/

#### UserName
- **Length**: 3-50 characters
- **Characters**: Letters, numbers, underscores, hyphens
- **First Character**: Must be a letter
- **Validation**: 
  - Regex pattern: /^[a-zA-Z][a-zA-Z0-9_-]*$/
  - Minimum 3 characters
  - Maximum 50 characters
  - Must start with letter
  - No consecutive special characters
  - No whitespace allowed

### Input Validation Patterns
- **Trim Whitespace**: All string inputs are automatically trimmed
- **Case Normalization**: Case-insensitive comparison for codes and identifiers
- **Length Validation**: Enforced on all string fields
- **Character Whitelisting**: Only allow specific character sets per field type
- **Regex Validation**: Comprehensive regex patterns for each field type
- **Format Validation**: Specific format validation for specialized fields (email, phone, NIF)
- **Range Validation**: For numeric values and length constraints
- **Uniqueness**: Enforced at database level for unique fields
- **Special Characters**: Strict control over allowed special characters
- **Pattern Matching**: Regex patterns for complex validation rules

### Data Sanitization

#### General Sanitization Rules
- **HTML/JavaScript**: All user input is automatically HTML-encoded by ASP.NET Core
- **SQL Injection**: Prevented through parameterized queries (Entity Framework Core)
- **XSS Protection**:
  - Input validation using whitelists
  - Output encoding in views
  - Content Security Policy (CSP) headers
- **SQL Injection Prevention**:
  - Parameterized queries
  - ORM protection
  - Input sanitization
- **CSRF Protection**: Built-in ASP.NET Core CSRF protection
- **Rate Limiting**: Protection against brute force attacks
- **Input Encoding**: Automatic encoding of special characters
- **Output Encoding**: Context-aware encoding for different output types

#### Input Validation Patterns
- **Trim Whitespace**: All string inputs are automatically trimmed
- **Case Normalization**: Case-insensitive comparison for codes and identifiers
- **Length Validation**: Enforced on all string fields
- **Character Whitelisting**: Only allow specific character sets per field type
- **Regex Validation**: Comprehensive regex patterns for each field type
- **Format Validation**: Specific format validation for specialized fields
- **Range Validation**: For numeric values and length constraints
- **Uniqueness**: Enforced at database level for unique fields
- **Special Characters**: Strict control over allowed special characters
- **Pattern Matching**: Regex patterns for complex validation rules

#### Output Sanitization
- **HTML Encoding**: For all string outputs
- **JavaScript Encoding**: For script contexts
- **URL Encoding**: For URL parameters
- **CSS Encoding**: For style contexts
- **Attribute Encoding**: For HTML attributes
- **SQL Parameterization**: For database queries
- **JSON Sanitization**: For API responses
- **XML Encoding**: For XML outputs
- **File Name Sanitization**: For file operations

#### Security Headers
- **Content Security Policy (CSP)**: Strict policy to prevent XSS
- **X-Content-Type-Options**: Prevent MIME type sniffing
- **X-Frame-Options**: Prevent clickjacking
- **Strict-Transport-Security**: Enforce HTTPS
- **X-XSS-Protection**: Enable browser XSS protection
- **Referrer-Policy**: Control referrer information
- **Permissions-Policy**: Restrict browser features
- **Cache-Control**: Prevent caching of sensitive data

## Data Sanitization

### General Sanitization Rules
- **HTML/JavaScript**: All user input is automatically HTML-encoded by ASP.NET Core
- **SQL Injection**: Prevented through parameterized queries (Entity Framework Core)
- **XSS Protection**:
  - Input validation using whitelists
  - Output encoding in views
  - Content Security Policy (CSP) headers

### Input Validation Patterns
- **Trim Whitespace**: All string inputs are automatically trimmed
- **Case Normalization**: Case-insensitive comparison for codes and identifiers
- **Length Validation**: Enforced on all string fields
- **Character Whitelisting**: Only allow specific character sets per field type
- **Regex Validation**: Comprehensive regex patterns for each field type
- **Format Validation**: Specific format validation for specialized fields
- **Range Validation**: For numeric values and length constraints
- **Uniqueness**: Enforced at database level for unique fields
- **Special Characters**: Strict control over allowed special characters
- **Pattern Matching**: Regex patterns for complex validation rules

### Book Data Sanitization
- **Titles and Names**:
  - Trimmed
  - Multiple spaces collapsed
  - Basic punctuation allowed
- **Descriptions**:
  - HTML tags stripped
  - Special characters encoded
  - Line breaks preserved but sanitized
- **ISBN**:
  - Hyphens and spaces removed
  - Converted to uppercase
  - Length and format validated

### Input Cleaning
- Automatic whitespace trimming on all string inputs
- Removal of potentially dangerous characters in free-text fields
- HTML tag stripping in descriptions

### XSS Prevention
- Input sanitization for all user-provided content
- HTML encoding for output
- Content Security Policy (CSP) headers in API responses

## Authentication & Authorization

### Keycloak Integration
- Centralized identity management
- OAuth 2.0 and OpenID Connect support
- Role-based access control (RBAC)
- Token-based authentication

### Session Management
- Secure cookie handling
- Token expiration and refresh mechanisms
- Secure token storage

## Data Protection

### Sensitive Data
- No sensitive data in logs
- Proper error handling without exposing system details
- Secure password hashing (handled by Keycloak)

### Database Security
- Parameterized queries to prevent SQL injection
- Principle of least privilege for database users
- Connection string security
- 
## API Security

### Authentication
- JWT Bearer Tokens for API authentication
- Token validation against Keycloak server
- Token expiration and refresh mechanisms

### Authorization
- Role-based access control (RBAC)
- Fine-grained permissions for different user roles
- Endpoint-level authorization attributes

### Request Validation
- Model validation using data annotations
- Custom validation attributes for complex rules
- Global exception handling for consistent error responses

### Rate Limiting
- Implemented to prevent abuse
- Configurable limits per endpoint
- IP-based rate limiting for unauthenticated endpoints

### Secure Headers
- CORS configured to allow specific origins
- HSTS enabled for HTTPS enforcement
- X-Content-Type-Options set to 'nosniff'
- X-Frame-Options set to 'DENY'
- X-XSS-Protection enabled with mode block
- Content-Security-Policy configured to restrict resources

## Logging & Monitoring

### Security Logging
- Authentication attempts (success/failure)
- Authorization failures
- Sensitive operations

## Email Security and Warning System

### Development Email Handling
- **Email Redirection**: All emails in development are redirected to a single development email address
- **Real User Protection**: Actual user emails are never used in development environment
- **Logging**: All email sending attempts and failures are logged for debugging

### Configuration
Email settings are configured in `appsettings.Development.json`:

```csharp
"Email": {
  "SmtpServer": "smtp.gmail.com",
  "Port": 587,
  "Username": "",  
  "Password": "", 
  "FromEmail": "noreply@yourdomain.com",
  "DevelopmentEmail": "dev-email@yourdomain.com",
  "EnableSsl": true
}
```

### Security Measures
- **No Hardcoded Credentials**: Email credentials are never committed to version control
- **Environment Variables**: Sensitive data is loaded from environment variables or user secrets
- **Development-Only**: The development email system is never active in production
- **Input Validation**: All email addresses are validated before processing

### Setting Up Environment Variables
For local development, use one of these methods:

1. **Environment Variables**:
   ```bash
   # Windows
   setx EMAIL_USERNAME "your-email@gmail.com"
   setx EMAIL_PASSWORD "your-app-specific-password"
   
   # Linux/macOS
   export EMAIL_USERNAME=your-email@gmail.com
   export EMAIL_PASSWORD=your-app-specific-password
   ```

2. **.NET User Secrets** (recommended):
   ```bash
   dotnet user-secrets set "Email:Username" "your-email@gmail.com"
   dotnet user-secrets set "Email:Password" "your-app-specific-password"
   ```

### Production Considerations
- The system automatically disables email redirection in production
- Production requires valid SMTP server configuration
- Email sending failures are logged and monitored
- Rate limiting is applied to prevent email abuse

## Best Practices

### Code Quality
- Static code analysis
- Code reviews
- Security-focused testing

