# Secure Development Requirements

> This section outlines the security-specific requirements to be followed throughout the software development lifecycle (SDLC). These are intended to reduce the attack surface, ensure data integrity, and enforce secure coding practices for the backend API and database system.

## Authentication and Access Control
- **SDR1.** User authentication must implement Multi-Factor Authentication (MFA) via a code sent by email, using Keycloak as the identity and access management system.
- **SDR2.** The application must lock the user account after three failed login attempts, as a preventive measure against brute force attacks.
- **SDR3.** User passwords must contain at least 8 characters, including uppercase and lowercase letters, numbers, and special characters.
- **SDR4.** Upon registration, the application must send a verification email to the user to validate their identity.
- **SDR5.** The system must implement Role-Based Access Control (RBAC), applying the principle of least privilege.
- **SDR6.** Authentication-related error messages must not be overly detailed to prevent the exposure of sensitive information to potential attackers.

## Data Protection
- **SDR7.** All sensitive data, both at rest and in transit, must be encrypted, ensuring secure communication using the TLS protocol.
- **SDR8.** Passwords must be securely hashed using algorithms such as bcrypt.
- **SDR9.** Collected personal data must be treated confidentially and used exclusively for their intended purposes, in compliance with the General Data Protection Regulation (GDPR).

## Input Validation & Error Handling
- **SDR10.** The application must validate user-submitted data, rejecting invalid values (e.g., nulls or incorrectly formatted data).
- **SDR11.** For the new book suggestion field, the application must only accept plain text, which must be validated and sanitized to prevent code injection, including SQL Injection and Cross-Site Scripting (XSS).

## Logging and Auditing
- **SDR12.** All logs of sensitive actions must be generated and stored in the system, with a copy also sent to an external logging server.
- **SDR13.** The system must perform three backups of the logs, with two stored in separate online systems and one in an offline location.


[Back to Documentation](../Documentation.md)
