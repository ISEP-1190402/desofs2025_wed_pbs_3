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
- **Characters**: Letters, numbers, spaces, and basic punctuation (.,:;!?()&'"-)
- **Case**: Case-insensitive comparison
- **Whitespace**: Automatic trimming
- **Security**: Basic XSS prevention through allowed character restrictions

#### ISBN
- **Formats**: ISBN-10 and ISBN-13
- **Input**: Accepts both hyphenated and non-hyphenated formats
- **Case**: Case-insensitive for 'X' in ISBN-10
- **No checksum validation** (as per requirements)
- **Length**: 10 or 13 digits (after removing hyphens)

#### Category
- **Length**: 2-50 characters
- **Characters**: Letters, spaces, and hyphens only (no numbers)
- **Case**: Case-insensitive comparison
- **Flexibility**: No predefined list, allowing library manager's discretion

#### Description
- **Length**: 5-2000 characters (reduced minimum from 10 to 5)
- **Security**: XSS protection through input sanitization
- **Content**: Allows standard text with basic punctuation
- **HTML**: Strips HTML tags to prevent XSS

#### Author
- **Length**: 2-100 characters
- **Characters**: Letters, spaces, hyphens (-), apostrophes ('), and periods (.) only (no numbers)
- **Whitespace**: Automatic trimming
- **Comparison**: Case-insensitive

#### Publisher
- **Length**: 2-100 characters
- **Characters**: Letters, numbers, spaces, and basic punctuation
- **Whitespace**: Automatic trimming
- **Comparison**: Case-insensitive

#### AmountOfCopies
- **Range**: 0 to 1500 (inclusive)
- **Type**: Integer values only
- **Null Safety**: Non-nullable

### User Domain

#### Email
- **Format**: Standard email validation
- **Case**: Case-insensitive
- **Length**: Reasonable length limits enforced

#### NIF (Portuguese Tax ID)
- **Format**: 9 digits
- **Validation**: Check digit validation
- **Special Characters**: None allowed

#### PhoneNumber
- **Format**: 9 digits (Portuguese format)
- **Optional**: Country code support
- **Separators**: Allows spaces, hyphens, dots

#### UserName
- **Length**: 3-50 characters
- **Characters**: Letters, numbers, underscores, hyphens
- **First Character**: Must be a letter

## Data Sanitization

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
## Logging & Monitoring

### Security Logging
- Authentication attempts (success/failure)
- Authorization failures
- Sensitive operations

## Best Practices

### Code Quality
- Static code analysis
- Code reviews
- Security-focused testing

