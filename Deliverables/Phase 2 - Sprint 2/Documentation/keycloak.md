# KEYCLOAK Configuration and Best Practices

This document outlines the deployment and security configurations of the Keycloak-based authentication system used in our Library Realm, hosted within a Docker container on an Azure VM. It details setup scripts, realm configurations, security practices, and compliance mappings to OWASP ASVS 4.0, with suggestions for future enhancements.

Link (ADMIN): https://keycloak-desofs3.westeurope.cloudapp.azure.com:8443/admin/master/console/#/library/

Link (Regular Account): https://keycloak-desofs3.westeurope.cloudapp.azure.com:8443/realms/library/account

## Deployment Environment
- Platform: Azure Virtual Machine
- Containerization: Docker Compose
- Keycloak Version: quay.io/keycloak/keycloak:latest (ASVS 4.0.1)
- Access: https://keycloak-desofs3.westeurope.cloudapp.azure.com:8443
- Database: MySQL 8.0 (keycloakmysql)
- HTTPS Enabled: Yes (self-signed cert via OpenSSL)
```csharp
sudo -i
cd /home/azureuser/
openssl req -x509 -newkey rsa:4096 -sha256 -days 365 \ 

  -nodes \ 

  -keyout tls.key \ 

  -out tls.crt \ 

  -subj "/CN=keycloak-desofs3.westeurope.cloudapp.azure.com" \ 

  -addext "subjectAltName=DNS:keycloak-desofs3.westeurope.cloudapp.azure.com" 


```
**Best Practice (ASVS 3.2.1) implemented:**

Import and install self-signed tls certificate to local machine as Trusted Root Certification Authorities:
```csharp
scp -i ./Downloads/keycloakvm_key.pem azureuser@4.231.97.121:/home/azureuser/keycloak-certs/tls.crt .
```

## Setup Commands Summary
**SSH:** ssh -i ./Downloads/ficheiro.pem azureuser@4.231.97.121

**Start library Realm:** ./init-keycloak.sh

**Environment variables in:** .env (all sensitive variables before execution)

**Cert path:** ./keycloak-certs/tls.key, tls.crt

**Docker:** docker compose up -d --build

## Realm Configuration: library
### Realm Roles
- **Admin:** Full control of realm.

- **LibraryManager:** Intermediate control, e.g., manage clients/users.

- **User:** Default for self-registered users.

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

- Flows Enabled:

        directAccessGrantsEnabled: enabled

        serviceAccountsEnabled: enabled

        authorizationServicesEnabled: enabled

        standardFlowEnabled: enabled

        implicitFlowEnabled: enabled

#### **Realm Settings - Email (SMTP) Configuration**

- **SMTP:** Gmail or Outlook SMTP

- **Port:** 587 (TLS)

- Enabled Features:
    - Authentication (APP password)
    - TLS

- Add Email From and Display Name

#### **Realm Settings - Required Actions**

- Enable VERIFY_EMAIL

- Click "Default Action" on VERIFY_EMAIL

- Enable CONFIGURE_TOTP

- Click "Default Action" on CONFIGURE_TOTP

**ASVS** Mappings:

 - 2.1.1 & 2.1.3 – Email verification

 - 2.1.4 – TOTP/MFA requirement

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

        Failure Threshold: 6

        Wait Increment: 60s

        Max Wait: 900s

#### Password Policies (ASVS 2.1.7)

Authentication - Policies

        Min length: >= 12
        Max length: <= 128

        Require at least 1: Uppercase, Digits, Special Character

        Password Expiry: 30 days
        
        Can't be username or email

        Password Reuse Prevention: not recently used in 30 days

#### Email Notification Triggers

Realm Settings - Login

- Enable **Verify Email**

Authentication - Flows - Account

- Enable Email Actions for user-triggered updates

#### Realm Settings - Events (ASVS 2.10.1, 2.10.2)

Events:
- Enable Event Logging
- Enable Admin Events

### Structure Keycloak Container

    /home/azureuser/
    ├── .env
    ├── docker-compose.yml
    ├── init-keycloak.sh
    ├── keycloak-certs/
    │   ├── tls.crt
    │   └── tls.key
    ├── init-keycloak-db.sql


### POSTMAN

For Token Retrieve
POST /realms/library/protocol/openid-connect/token

        {
        "client_id": "library-client",
        "client_secret": "...",
        "username": "admin",
        "password": "...",
        "grant_type": "password"
        }

### Security Features

| Feature                    | Description                                  | ASVS Mapping   |
| -------------------------- | -------------------------------------------- | -------------- |
| **Email Verification**     | On registration                              | 2.1.1, 2.1.3   |
| **TOTP/MFA**               | Default action & enforced in flow            | 2.1.4          |
| **Strong Password Policy** | Min length, uppercase, digits, special chars | 2.1.7          |
| **Account Lockout**        | Brute-force protection                       | 2.1.6, 2.8.1   |
| **Admin/API Logging**      | Events and Admin Logs                        | 2.10.1, 2.10.2 |

### Best Practices Mapped to OWASP ASVS

| Control             | Description                                              |
| ------------------- | -------------------------------------------------------- |
| **2.1.1 / 2.1.3**   | Verified emails before granting access                   |
| **2.1.4**           | Configured TOTP/MFA for all users                        |
| **2.1.6 / 2.8.1**   | Brute force protection: Lock after failed attempts attempts     |
| **2.1.7**           | Strong password policy (length, complexity, expiration)  |
| **2.10.1 / 2.10.2** | Enabled event logging and admin events                   |
| **2.9.1**           | Self-signed cert is not trusted; better to use real cert |
