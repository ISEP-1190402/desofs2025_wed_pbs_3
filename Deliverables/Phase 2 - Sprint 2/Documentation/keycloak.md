# Keycloak Configuration and Management

This document provides comprehensive documentation for the Keycloak identity and access management system used in our application. It covers the initialization script, realm configuration, security settings, and best practices for managing the Keycloak instance.

## Table of Contents
- [Deployment Environment](#deployment-environment)
- [Initialization Script](#initialization-script)
- [Realm Configuration](#realm-configuration)
- [Security Settings](#security-settings)
- [Client Configuration](#client-configuration)
- [User Management](#user-management)
- [Application Integration](#application-integration)
- [Troubleshooting](#troubleshooting)
- [Best Practices](#best-practices)

## Initialization Script

The `init-keycloak.sh` script automates the setup and configuration of Keycloak. It performs the following actions:

### 1. Environment Setup
- Loads environment variables from `.env` file
- Validates required environment variables
- Sets up error handling

### 2. Authentication
- Obtains admin token using credentials from environment variables
- Validates successful authentication

### 3. Realm Creation
- Creates the `library` realm if it doesn't exist
- Defines default roles:
  - Admin: Full administrative privileges
  - LibraryManager: Book and rental management
  - User: Standard user access
- Sets the default role for new users to "User"

### 4. Client Configuration
- Creates `library-client` with the following settings:
  - Client type: Confidential
  - Authentication: Client secret based
  - Redirect URIs: `http://localhost:8081/*`
  - Web Origins: `http://localhost:8081`
  - Enabled flows:
    - Standard Flow (OAuth 2.0 Authorization Code)
    - Implicit Flow
    - Direct Access Grants
    - Service Accounts

### 5. Admin User Setup
- Creates an admin user with the following details:
  - Username: admin
  - Email: examplegogsi@gmail.com
  - Email verification: Enabled
  - Password: From environment variable
- Assigns the Admin role to the admin user

### 6. Security Policies

#### Password Policy
- Minimum 12 characters
- At least 1 uppercase letter
- At least 1 digit
- At least 1 special character
- Username not allowed in password
- Email not allowed in password
- Password history (last 5 passwords remembered)

#### Brute Force Protection
- Enabled with 6 failed attempts
- 60-second wait increment
- 15-minute maximum lockout
- No permanent lockout (auto-unlock)

### 7. Required Actions
- **Email Verification**: Required for all new users
- **TOTP (2FA)**: Required for all users
- Both actions are set as default actions

### 8. User Profile Configuration
- First Name: Not required
- Last Name: Not required
- Email: Required and must be verified
- Username: Required

### 9. SMTP Configuration
- Server: smtp.gmail.com:587
- Encryption: STARTTLS
- From: examplegogsi@gmail.com
- Display Name: Library-desofs-3
- Authentication: Username/Password

### 10. Token Configuration
- Access Token Lifespan: 2 hours
- Refresh Token Lifespan: 30 days
- Token Format: JWT (JSON Web Token)
- Signature Algorithm: RS256

### Usage
```bash
# Make the script executable
chmod +x init-keycloak.sh

# Run the script
./init-keycloak.sh
```

### Environment Variables
Required variables in `.env` file:
```
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=your_admin_password
KEYCLOAK_CLIENT_SECRET=your_client_secret
SMTP_PASSWORD=your_smtp_password
```

## Deployment Environment

### System Requirements
- **Platform**: Windows Server 2022
- **Java**: JDK 17 or later
- **Database**: MySQL 8.0+
- **Memory**: Minimum 2GB RAM
- **Ports**: 8080 (HTTP), 8443 (HTTPS)
- **Keycloak Version**: 26.2.5

### Network Configuration
- **Access**: http://localhost:8080
- **HTTPS**: Disabled (for development only)
- **Access Restrictions**: Internal network only

## Realm Configuration

### Library Realm
- **Realm Name**: library
- **Display Name**: Library Management System
- **Enabled**: Yes
- **User Registration**: Disabled
- **Email Verification**: Required (ASVS 2.1.3)
- **Login with Email**: Disabled
- **Default Role**: "User" (automatically assigned to new users)
  - Configured via the realm's `defaultRoles` setting
  - Ensures all new users have basic access rights
  - Applied during user registration or creation
- **Password Policy**:
  - Minimum length: 12 characters
  - Maximum length: 128 characters
  - At least 1 uppercase letter, 1 digit, and 1 special character
  - Cannot contain username or email
  - Password history: 5 previous passwords remembered
  - Expiry: 60 days

### Authentication Flows

#### Browser Flow
- Username/Password Form - Required
- OTP Form - Required
- Set as default browser flow

#### Registration Flow (ASVS 2.1.3)
- **Verify Email** execution - Required
  - Ensures email verification for new registrations

### Roles and Mappings
1. **Admin**
   - Full administrative access
   - Manages users, roles, and realm settings
   - Assigned to initial admin user

2. **LibraryManager**
   - Manages books and users
   - Restricted administrative access

3. **User**
   - Regular user access
   - Can view and borrow books

### Client Scope: roles
- **Type**: Default
- **Protocol**: OpenID Connect
- **Description**: Include user roles in tokens
- **Mapper**: User Realm Role
  - Token Claim Name: roles
  - Claim Type: String
  - Add to ID token: Yes
  - Add to access token: Yes
  - Add to userinfo: Yes
  - Multivalued: Yes

### Security Settings
- **Brute Force Protection**:
  - Enabled: Yes
  - Permanent Lockout: No
  - Maximum Login Failures: 5
  - Wait Increment: 60 seconds
  - Maximum Wait: 600 seconds
  - Minimum Quick Login Wait: 60 seconds
  - Maximum Delta Time: 43200 seconds (12 hours)

- **Token Settings**:
  - Access Token Lifespan: 7200 seconds (2 hours)
  - SSO Session Idle: 1800 seconds (30 minutes)
  - SSO Session Max: 28800 seconds (8 hours)
  - Offline Session Idle: 2592000 seconds (30 days)
  - User Info Signed Response Algorithm: RS256

## Security Settings

### Password Policy
- Minimum length: 12 characters
- Maximum length: 128 characters
- At least 1 uppercase letter
- At least 1 digit
- At least 1 special character
- Cannot contain username
- Cannot contain email
- Password history: 5 previous passwords remembered
- Password expiry: 60 days

### Brute Force Protection
- Enabled: Yes
- Permanent Lockout: No
- Maximum Login Failures: 5
- Wait Increment: 60 seconds
- Maximum Wait: 600 seconds
- Minimum Quick Login Wait: 60 seconds
- Maximum Delta Time: 43200 seconds (12 hours)

### Token Settings
- Access Token Lifespan: 7200 seconds (2 hours)
- Refresh Token Lifespan: 2592000 seconds (30 days)
- SSO Session Idle: 1800 seconds (30 minutes)
- SSO Session Max: 28800 seconds (8 hours)
- User Info Signed Response Algorithm: RS256
- Token Format: JWT (JSON Web Token)
- Signature Algorithm: RS256

## Client Configuration

### Library Client
- **Client ID**: library-client
- **Protocol**: OpenID Connect
- **Access Type**: confidential
- **Authentication**: Client secret based
- **Redirect URIs**: http://localhost:8081/*
- **Web Origins**: http://localhost:8081
- **Enabled Flows**:
  - Standard Flow (OAuth 2.0 Authorization Code)
  - Implicit Flow
  - Direct Access Grants
  - Service Accounts

### Client Scope: roles
- **Type**: Default
- **Protocol**: OpenID Connect
- **Description**: Include user roles in tokens
- **Mapper**: User Realm Role
  - Token Claim Name: roles
  - Claim Type: String
  - Add to ID token: Yes
  - Add to access token: Yes
  - Add to userinfo: Yes
  - Multivalued: Yes

## User Management

### Default Users
1. **Admin User**:
   - Username: admin
   - Email: examplegogsi@gmail.com
   - Roles: Admin
   - Email verified: Yes
   - Password: Set via environment variable

### User Profile Configuration
- **First Name**: Not required
- **Last Name**: Not required
- **Email**: Required and must be verified
- **Username**: Required, 3-50 characters, alphanumeric with underscores/hyphens

### SMTP Configuration
- **Server**: smtp.gmail.com:587
- **Encryption**: STARTTLS
- **From**: examplegogsi@gmail.com
- **Display Name**: Library-desofs-3
- **Authentication**: Username/Password

## Troubleshooting

### Common Issues
1. **Connection Refused**
   - Ensure Keycloak is running
   - Check if port 8080 is open
   - Verify firewall settings

2. **Authentication Failures**
   - Verify admin credentials
   - Check if the admin user is enabled
   - Verify realm settings

3. **Database Connection Issues**
   - Verify MySQL is running
   - Check database credentials in keycloak.conf
   - Ensure the database user has proper permissions

4. **Email Not Sending**
   - Verify SMTP settings
   - Check if SMTP server allows relay
   - Verify SMTP credentials

## Best Practices

### Security
- Always use HTTPS in production
- Regularly rotate admin credentials
- Enable audit logging
- Keep Keycloak updated
- Regular database backups

### Performance
- Enable database connection pooling
- Configure appropriate JVM settings
- Use caching where appropriate
- Monitor system resources

### Maintenance
- Regular log reviews
- System monitoring
- Documentation updates
- Backup testing

## Prerequisites
1. Azure VM (Windows Server 2022)
2. MySQL Server installed and running locally
3. Java 17 installed
4. Keycloak 26.2.5 [downloaded](https://github.com/keycloak/keycloak/releases/download/26.2.5/keycloak-26.2.5.zip) and extracted to `C:\Program Files\keycloak-26.2.5`

## Database Setup
1. Create the Keycloak database:
```sql
DROP DATABASE IF EXISTS keycloak;
CREATE DATABASE keycloak CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```
## Keycloak Configuration
1. Edit `keycloak.conf` in the Keycloak `conf` directory with the following settings:
```
db=mysql
db-username=desofs
db-password=your_db_password
db-url=jdbc:mysql://localhost:3306/keycloak
```
## Application Integration

### Backend Configuration (Startup.cs)
```csharp
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = Configuration["Keycloak__Authority"];
    options.Audience = Configuration["Keycloak__Audience"];
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

### Admin Token Acquisition
```csharp
private async Task<string> GetAdminTokenAsync() 
{
    var authority = Environment.GetEnvironmentVariable("Keycloak__Authority").TrimEnd('/');
    var keycloakUrl = authority.Replace("/realms/library", "");
    var content = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("grant_type", "password"),
        new KeyValuePair<string, string>("client_id", "admin-cli"),
        new KeyValuePair<string, string>("username", Environment.GetEnvironmentVariable("Keycloak__Username")),
        new KeyValuePair<string, string>("password", Environment.GetEnvironmentVariable("Keycloak__Password")),
    });
    // Token request implementation...
}
```

### Required Environment Variables
Set these at the system level for global availability:

```
# Keycloak Admin Credentials
KEYCLOAK_ADMIN=desofs-kc
KEYCLOAK_ADMIN_PASSWORD=your_admin_password

# Application Configuration
Keycloak__Authority=http://localhost:8080/realms/library
Keycloak__Audience=library-client
Keycloak__ClientId=library-client
Keycloak__ClientSecret=your_client_secret
Keycloak__URL=http://localhost:8080
```

### Security Notes
- **HTTPS**: Always use HTTPS in production
- **Environment Variables**: Store sensitive data in environment variables, not in code
- **Token Security**: Implement proper token storage and handling
- **Error Handling**: Add comprehensive error handling for authentication failures
```
These other variables are consumed by the backend API application (UserService.cs and Startup.cs) to authenticate and retrieve an admin token, which is required to invoke administrative Keycloak endpoints (e.g., for user creation).

## Automated Startup Script
Create a batch file (e.g., `start-keycloak.bat`) with the following content:
```batch
@echo off
cd /d "C:\Program Files\keycloak-26.2.5\bin"
start "" /min kc.bat start-dev 
```

# Script Init-Keycloak.sh

All the realm, client, user, email, and security settings are automatically applied by the `Init-Keycloak.sh` script executed inside the container or VM after Keycloak has started.

## Keycloak Initialization Logic (Shell Script Overview)

### Prerequisites
- Keycloak server running on port 8080
- `jq` installed for JSON processing
- Environment variables set:
  - `KEYCLOAK_ADMIN`: Admin username
  - `KEYCLOAK_ADMIN_PASSWORD`: Admin password
  - `KEYCLOAK_CLIENT_SECRET`: Client secret for library-client
  - `SMTP_PASSWORD`: SMTP password for email

### 1. Get Admin Token
This authenticates with the master realm and retrieves an access token for subsequent API calls.

```
ADMIN_TOKEN=$(curl -X POST http://localhost:8080/realms/master/protocol/openid-connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "username=${KEYCLOAK_ADMIN}" \
  -d "password=${KEYCLOAK_ADMIN_PASSWORD}" \
  -d "grant_type=password" \
  -d "client_id=admin-cli" | jq -r '.access_token')
```
### 2. Update Master Realm Admin Info
Updates the admin user's email and marks it as verified.
```
MASTER_ADMIN_ID=$(curl -s -X GET "http://localhost:8080/admin/realms/master/users?username=${KEYCLOAK_ADMIN}" \
  -H "Authorization: Bearer $ADMIN_TOKEN" | jq -r '.[0].id')

curl -X PUT "http://localhost:8080/admin/realms/master/users/${MASTER_ADMIN_ID}" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "examplegogsi@gmail.com",
    "emailVerified": true
  }'
```
### 3. Create and Assign Admin User
Creates the initial admin user in the library realm and assigns the Admin role.
```
curl -X POST http://localhost:8080/admin/realms/library/users \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "email": "examplegogsi@gmail.com",
    "enabled": true,    
    "emailVerified": true,
    "credentials": [{
      "type": "password",
      "value": "'${KEYCLOAK_ADMIN_PASSWORD}'",
      "temporary": false
    }]
  }'

ADMIN_USER_ID=$(curl -X GET "http://localhost:8080/admin/realms/library/users?username=admin" \
  -H "Authorization: Bearer $ADMIN_TOKEN" | jq -r '.[0].id')

ADMIN_ROLE=$(curl -H "Authorization: Bearer $ADMIN_TOKEN" \
  "http://localhost:8080/admin/realms/library/roles/Admin")

curl -X POST "http://localhost:8080/admin/realms/library/users/${ADMIN_USER_ID}/role-mappings/realm" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d "[$ADMIN_ROLE]"

```

## 4. Realm Configuration: library
### 4.1 Realm Roles
Defines the three main roles for the application:
- **Admin**: Full administrative access
- **LibraryManager**: Manages library resources
- **User**: Default role for all authenticated users
- **Admin:** Full control of realm.

- **LibraryManager:** Intermediate control, e.g., manage clients/users.

- **User:** Default for self-registered users.
```
curl -X POST http://localhost:8080/admin/realms \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "realm": "library",
    "enabled": true,
    "roles": {
      "realm": [
        { "name": "Admin", "description": "Administrator role" },
        { "name": "LibraryManager", "description": "Library Manager role" },
        { "name": "User", "description": "Regular user role" }
      ]
    }
  }'

until curl -H "Authorization: Bearer $ADMIN_TOKEN" \
  "http://localhost:8080/admin/realms/library/roles/Admin" | grep -q '"name":"Admin"'; do
    sleep 2
done

```
### Configuration

### 4.2 Realm Settings

- **HTTPS Required:** All external traffic (port 8443)
- **Default Role:** User
- **Token lifespan:** 2h
- **Self-Registration:** Enabled
- **Login with Email:** Disabled
- **First/Last Name Required:** Removed

```bash
# Disable registration and login with email
curl -X PUT "http://localhost:8080/admin/realms/library" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "registrationAllowed": true,
    "loginWithEmailAllowed": false
  }'
```

### 4.3 Client Configuration
***library-client***
- clientAuthenticatorType: client-secret
```
curl -X POST http://localhost:8080/admin/realms/library/clients \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": "library-client",
    "secret": "'${KEYCLOAK_CLIENT_SECRET}'",
    "redirectUris": ["http://localhost:8081/*"],
    "webOrigins": ["http://localhost:8081"],
    "publicClient": false,
    "directAccessGrantsEnabled": true,
    "serviceAccountsEnabled": true,
    "authorizationServicesEnabled": true,
    "standardFlowEnabled": true,
    "implicitFlowEnabled": true,
    "protocol": "openid-connect",
    "clientAuthenticatorType": "client-secret"
  }'
```

### 4.4 Email (SMTP) Configuration

- **SMTP:** Gmail or Outlook SMTP

- **Port:** 587 (TLS)

- Enabled Features:
    - Authentication (APP password)
    - TLS

- Add Email From and Display Name

```
curl -X PUT "http://localhost:8080/admin/realms/library" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "accessTokenLifespan": 7200,
    "verifyEmail": true,
    "smtpServer": {
      "host": "smtp.gmail.com",
      "port": "587",
      "from": "examplegogsi@gmail.com",
      "fromDisplayName": "Library-desofs-3",
      "auth": true,
      "user": "examplegogsi@gmail.com",
      "password": "'${SMTP_PASSWORD}'",
      "ssl": false,
      "starttls": true
    }
  }'
```
### 4.5 Required Actions
Configures mandatory user actions:
- Email Verification (ASVS 2.1.1, 2.1.3)
- TOTP Configuration (ASVS 2.1.4)

- Enable VERIFY_EMAIL

- Click "Default Action" on VERIFY_EMAIL

- Enable CONFIGURE_TOTP

- Click "Default Action" on CONFIGURE_TOTP

**ASVS** Mappings:

- 2.1.1 & 2.1.3 – Email verification

- 2.1.4 – TOTP/MFA requirement

```
REQUIRED_ACTIONS=$(curl -s -H "Authorization: Bearer $ADMIN_TOKEN" \
  "http://localhost:8080/admin/realms/library/authentication/required-actions")

echo "$REQUIRED_ACTIONS" | jq -c '.[]' | while read -r action; do
  ALIAS=$(echo "$action" | jq -r '.alias')
  if [[ "$ALIAS" == "VERIFY_EMAIL" || "$ALIAS" == "CONFIGURE_TOTP" ]]; then
    UPDATED=$(echo "$action" | jq '.enabled = true | .defaultAction = true')
  else
    UPDATED=$(echo "$action" | jq '.enabled = false | .defaultAction = false')
  fi

  curl -s -X PUT "http://localhost:8080/admin/realms/library/authentication/required-actions/$ALIAS" \
    -H "Authorization: Bearer $ADMIN_TOKEN" \
    -H "Content-Type: application/json" \
    -d "$UPDATED"
done
```

#### **Realm Settings - Brute Force Protection (ASVS 2.1.6 / 2.8.1)**

Security Defenses - Brute Force Detection

- Enable Brute Force Detection

- Set:
  ```
  curl -X PUT "http://localhost:8080/admin/realms/library" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "bruteForceProtected": true,
    "permanentLockout": false,
    "failureFactor": 5,
    "waitIncrementSeconds": 60,
    "maxFailureWaitSeconds": 600,
    "minimumQuickLoginWaitSeconds": 60,
    "maxDeltaTimeSeconds": 43200
  }'
 ```
```
### 4.7 Password Policies (ASVS 2.1.7)
Enforces strong password requirements:
- Minimum 12 characters
- Maximum 128 characters
- At least 1 uppercase, 1 digit, 1 special character
- No username or email in password
- Password history (last 5 passwords) (ASVS 2.1.7)

Authentication - Policies
```
    UPDATED_JSON=$(echo "$REALM_JSON" | jq '
  .passwordPolicy = "length(12) and maxLength(128) and upperCase(1) and digits(1) and specialChars(1) and notUsername() and notEmail() and passwordHistory(5)"
')

curl -X PUT "http://localhost:8080/admin/realms/library" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d "$UPDATED_JSON"
```

### 4.8 Event Logging (ASVS 2.10.1, 2.10.2)

Configures comprehensive event logging for security monitoring:


```
# Enable event logging
curl -X PUT "http://localhost:8080/admin/realms/library" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "eventsEnabled": true,
    "adminEventsEnabled": true,
    "adminEventsDetailsEnabled": true,
    "eventsListeners": ["jboss-logging"],
    "enabledEventTypes": [
      "LOGIN",
      "LOGIN_ERROR",
      "REGISTER",
      "UPDATE_PROFILE",
      "UPDATE_EMAIL",
      "UPDATE_PASSWORD",
      "SEND_RESET_PASSWORD",
      "SEND_VERIFY_EMAIL",
      "REMOVE_TOTP",
      "VERIFY_EMAIL",
      "DELETE_ACCOUNT"
    ],
    "eventsExpiration": 0  # Never expire events
  }'

# Set up event logging retention
curl -X PUT "http://localhost:8080/admin/realms/library/events/config" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "enabled": true,
    "enabledEventTypes": ["*"],
    "adminEventsEnabled": true,
    "adminEventsDetailsEnabled": true,
    "eventsExpiration": 0
  }'

```
### 4.9 UI Themes
Configures the visual appearance of Keycloak interfaces:
- Login Theme: keycloak
- Account Theme: keycloak.v3
- Admin Theme: keycloak.v2
- Email Theme: keycloak
Needed to perform test of  configuration of OTP and the verification of email
```
UPDATED_JSON=$(echo "$REALM_JSON" | jq '
  .loginTheme = "keycloak" |
  .accountTheme = "keycloak.v3" |
  .adminTheme = "keycloak.v2" |
  .emailTheme = "keycloak"
')

curl -s -X PUT "http://localhost:8080/admin/realms/library" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d "$UPDATED_JSON"
```

#### MANUAL SETTINGS needed

### User Profile Configuration
- **Realm Settings** → **User Profile**
  - Removed `firstName` and `lastName` attributes from the user profile
  - Only essential fields are kept for user management

### Authentication Flows
- **Browser Flow**
  - Username/Password Form: Required
  - OTP Form: Required
  - Set as default browser flow

- **Registration Flow** (ASVS 2.1.3)
  - Added Verify Email execution (Required)
  - Ensures email verification for new registrations

### Client Scope: roles
- **Type**: Default
- **Protocol**: OpenID Connect
- **Description**: Include user roles in tokens
- **Mapper**: User Realm Role
  - Token Claim Name: roles
  - Claim Type: String
  - Add to ID token: Yes
  - Add to access token: Yes
  - Add to userinfo: Yes
  - Multivalued: Yes

### SSO SESSION
The same as Access Token - 2h.

### Password Policy
- Password expiry set to 60 days
- Client scopes for ROLES are configured to ensure they appear in the token
- 
POST /realms/library/protocol/openid-connect/token
```
        {
        "client_id": "library-client",
        "client_secret": "...",
        "username": "...",
        "password": "...",
        "grant_type": "password"
        }
  ```      
Note: Client library-client is public, then there is no secret.

### Security Features

| Feature                    | Description                                   | ASVS Mapping   |
| -------------------------- | --------------------------------------------- | -------------- |
| **Email Verification**     | Enabled via required actions and email config | 2.1.1, 2.1.3   |
| **TOTP (2FA)**             | Enabled & set as default required action      | 2.1.4          |
| **Strong Password Policy** | Enforced: length, complexity, no reuse        | 2.1.7          |
| **Brute Force Protection** | Lockouts & wait time policy                   | 2.1.6, 2.8.1   |
| **Admin & Event Logging**  | Admin and user actions tracked                | 2.10.1, 2.10.2 |
| **Self-Registration**      | Enabled. Requires email verification         | 2.1.1          |


### Best Practices Mapped to OWASP ASVS

| Control             | Description                                              |
| ------------------- | -------------------------------------------------------- |
| **2.1.1 / 2.1.3**   | Verified emails before granting access                   |
| **2.1.4**           | Configured TOTP/MFA for all users                        |
| **2.1.6 / 2.8.1**   | Brute force protection: Lock after failed attempts attempts     |
| **2.1.7**           | Strong password policy (length, complexity, expiration)  |
| **2.10.1 / 2.10.2** | Enabled event logging and admin events                   |
| **2.9.1**           | Self-signed cert is not trusted; better to use real cert |
