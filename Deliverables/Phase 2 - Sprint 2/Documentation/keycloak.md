# KEYCLOAK Configuration and Best Practices

This document outlines the deployment and security configurations of the Keycloak-based authentication system used in our Library Realm, installed on a Windows Server 2022 Azure VM. It details setup scripts, realm configurations, security practices, and compliance mappings to OWASP ASVS 4.0, with suggestions for future enhancements.

## Access URLs
- **Keycloak Admin Console**: http://localhost:8080/admin/
- **Account Console**: http://localhost:8080/realms/library/account/

## Deployment Environment
- **Platform**: Azure Virtual Machine (Windows Server 2022)
- **VM Name**: vm-desofs2025-wed-pbs-3
- **Operating System**: Windows Server 2022 Datacenter Azure Edition
- **Keycloak Version**: 26.2.5
- **Database**: MySQL 8.0 (Separate databases for application and Keycloak)
- **Access**: http://localhost:8080
- **HTTPS Enabled**: No (HTTP only)
- **Access Restrictions**: Keycloak is only accessible internally (not exposed externally)

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
db-password=E*bCRJ_ijo*N3u_kMMZb
db-url=jdbc:mysql://localhost:3306/keycloak
```

## Application Integration Details

#### Startup.cs Configuration: 
This setup configures the backend to authenticate against Keycloak using JWT.
It relies on the Keycloak__Authority and Keycloak__Audience environment variables to validate tokens issued by the Keycloak server.

```
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
#### UserService.cs – Retrieving the Admin Token
This method programmatically requests an access token for the admin user (desofs-kc) from Keycloak.

Without properly defined environment variables, the request will fail with "invalid_grant", as credentials will be missing or incorrect.
```
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

    // Token request to Keycloak goes here
}

```

This method programmatically requests an access token for the admin user (desofs-kc) from Keycloak.

**Unlike the configuration system used in Startup.cs, this code uses Environment.GetEnvironmentVariable(...) to read credentials directly from the operating system’s environment. If these variables (Keycloak__Username, Keycloak__Password) are not properly set at the system level, the method will receive null values for both. As a result, the token request will be sent with missing credentials, leading Keycloak to respond with an invalid_grant error.** This is why it’s essential to define the variables correctly and ensure they are available in the environment context in which the application runs.


### Environment Variables and Application Integration

Setting these environment variables at the system level ensures they are available globally and persist across reboots. This:

- **Enables automatic token acquisition required for secured API operations (e.g., user creation).**
- Keeps sensitive data like passwords outside of source code, reducing security risks.
- Ensures Keycloak’s CLI-based admin user initialization works on every restart.

The first two variables (KEYCLOAK_ADMIN, KEYCLOAK_ADMIN_PASSWORD) are used when launching kc.bat start-dev to bootstrap the initial admin account under the master realm.
```
KEYCLOAK_ADMIN desofs-kc
KEYCLOAK_ADMIN_PASSWORD xc.uUrqxbz6tDYyQryhK
```
In order to enable seamless integration between the backend application and Keycloak, critical environment variables were set at the system level on the Windows Server 2022 VM. These variables are used both during Keycloak startup and within the application to authenticate and manage users programmatically.
Set on the Windows system environment:
```
Keycloak__Username=desofs-kc
Keycloak__Password=xc.uUrqxbz6tDYyQryhK
Keycloak__Audience=library-client
Keycloak__Authority=http://localhost:8080/realms/library
Keycloak__ClientId=library-client
Keycloak__URL=http://localhost:8080
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

### Get Admin Token

```
ADMIN_TOKEN=$(curl -X POST http://localhost:8082/realms/master/protocol/openid-connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "username=${KEYCLOAK_ADMIN}" \
  -d "password=${KEYCLOAK_ADMIN_PASSWORD}" \
  -d "grant_type=password" \
  -d "client_id=admin-cli" | jq -r '.access_token')
```
### Update Master Realm Admin Info
```
MASTER_ADMIN_ID=$(curl -s -X GET "http://localhost:8082/admin/realms/master/users?username=${KEYCLOAK_ADMIN}" \
  -H "Authorization: Bearer $ADMIN_TOKEN" | jq -r '.[0].id')

curl -X PUT "http://localhost:8082/admin/realms/master/users/${MASTER_ADMIN_ID}" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "examplegogsi@gmail.com",
    "emailVerified": true
  }'
```
### Create and Assign Admin User
```
curl -X POST http://localhost:8082/admin/realms/library/users \
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

ADMIN_USER_ID=$(curl -X GET "http://localhost:8082/admin/realms/library/users?username=admin" \
  -H "Authorization: Bearer $ADMIN_TOKEN" | jq -r '.[0].id')

ADMIN_ROLE=$(curl -H "Authorization: Bearer $ADMIN_TOKEN" \
  "http://localhost:8082/admin/realms/library/roles/Admin")

curl -X POST "http://localhost:8082/admin/realms/library/users/${ADMIN_USER_ID}/role-mappings/realm" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d "[$ADMIN_ROLE]"

```

## Realm Configuration: library
### Realm Roles
- **Admin:** Full control of realm.

- **LibraryManager:** Intermediate control, e.g., manage clients/users.

- **User:** Default for self-registered users.
```
curl -X POST http://localhost:8082/admin/realms \
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
  "http://localhost:8082/admin/realms/library/roles/Admin" | grep -q '"name":"Admin"'; do
    sleep 2
done

```
### Configuration

#### **Realm Settings**

- **HTTPS Required:** All external traffic (port 8443)

- **Default Role:** User

- **Token lifespan:** 2h

- **Self-Registration Enabled:** enabled

- **First/Last Name Required:** removed

#### **Clients**
***library-client***
- clientAuthenticatorType: client-secret
```
curl -X POST http://localhost:8082/admin/realms/library/clients \
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

#### **Realm Settings - Email (SMTP) Configuration**

- **SMTP:** Gmail or Outlook SMTP

- **Port:** 587 (TLS)

- Enabled Features:
    - Authentication (APP password)
    - TLS

- Add Email From and Display Name

```
curl -X PUT "http://localhost:8082/admin/realms/library" \
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
#### **Realm Settings - Required Actions**

- Enable VERIFY_EMAIL

- Click "Default Action" on VERIFY_EMAIL

- Enable CONFIGURE_TOTP

- Click "Default Action" on CONFIGURE_TOTP

**ASVS** Mappings:

 - 2.1.1 & 2.1.3 – Email verification

 - 2.1.4 – TOTP/MFA requirement

```
REQUIRED_ACTIONS=$(curl -s -H "Authorization: Bearer $ADMIN_TOKEN" \
  "http://localhost:8082/admin/realms/library/authentication/required-actions")

echo "$REQUIRED_ACTIONS" | jq -c '.[]' | while read -r action; do
  ALIAS=$(echo "$action" | jq -r '.alias')
  if [[ "$ALIAS" == "VERIFY_EMAIL" || "$ALIAS" == "CONFIGURE_TOTP" ]]; then
    UPDATED=$(echo "$action" | jq '.enabled = true | .defaultAction = true')
  else
    UPDATED=$(echo "$action" | jq '.enabled = false | .defaultAction = false')
  fi

  curl -s -X PUT "http://localhost:8082/admin/realms/library/authentication/required-actions/$ALIAS" \
    -H "Authorization: Bearer $ADMIN_TOKEN" \
    -H "Content-Type: application/json" \
    -d "$UPDATED"
done
```

#### **Authentication Flows**

Authentication - Flows - Browser with Email First

- Username Password Form - **Required**

- OTP Form - **Required**

- Set to **Bind Flow with Browser** (to become default browser flow)

Authentication - Flow - Registration

- Add Execution: **Verify Email**

- Requirement: **Required**

#### **Realm Settings - Brute Force Protection (ASVS 2.1.6 / 2.8.1)**

Security Defenses - Brute Force Detection

- Enable Brute Force Detection

- Set:
  ```
  curl -X PUT "http://localhost:8082/admin/realms/library" \
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
#### Password Policies (ASVS 2.1.7)

Authentication - Policies
```
    UPDATED_JSON=$(echo "$REALM_JSON" | jq '
  .passwordPolicy = "length(12) and maxLength(128) and upperCase(1) and digits(1) and specialChars(1) and notUsername() and notEmail() and passwordHistory(5)"
')

curl -X PUT "http://localhost:8082/admin/realms/library" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d "$UPDATED_JSON"
```

#### Email Notification Triggers

Realm Settings - Login

- Enable **Verify Email**

Authentication - Flows - Account

- Enable Email Actions for user-triggered updates

#### Realm Settings - Events (ASVS 2.10.1, 2.10.2)

Events:
- Enable Event Logging
- Enable Admin Events
#### Themes
Needed to perform test of  configuration of OTP and the verification of email
```
UPDATED_JSON=$(echo "$REALM_JSON" | jq '
  .loginTheme = "keycloak" |
  .accountTheme = "keycloak.v3" |
  .adminTheme = "keycloak.v2" |
  .emailTheme = "keycloak"
')

curl -s -X PUT "http://localhost:8082/admin/realms/library" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d "$UPDATED_JSON"
```

#### MANUAL SETTINGS needed

Authentication FLOWS
Associated roles to role Admin to give full control
Expiry password set to 60 days
Add client scopes for ROLES to make sure they appear in the token

### POSTMAN

For Token Retrieve
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
Note: If client library-client is public, then there is no secret.

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
