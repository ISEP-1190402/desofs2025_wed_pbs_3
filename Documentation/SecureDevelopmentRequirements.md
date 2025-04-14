# Secure Development Requirements

> This section outlines the security-specific requirements to be followed throughout the software development lifecycle (SDLC). These are intended to reduce the attack surface, ensure data integrity, and enforce secure coding practices for the backend API and database system.

## Authentication and Access Control
- **SDR1.** The system must enforce strong password policies.
- **SDR2.** All user accounts must use multi-factor authentication (MFA) during login.
- **SDR3.** The application must implement Role-Based Access Control (RBAC) with clearly defined permissions for each role.
- **SDR4.** Access to sensitive endpoints must be restricted based on the user’s role.
- **SDR5.** All failed login attempts must be rate-limited and logged.

## Data Protection
- **SDR6.** All communications must use TLS/HTTPS to ensure data is encrypted in transit.
- **SDR7.** Sensitive information (e.g., passwords, tokens) must never be logged or stored in plaintext.
- **SDR8.** Passwords must be hashed using a secure algorithm.
- **SDR9.** Uploaded files must be scanned and validated for type and size to prevent injection attacks.

## Input Validation & Error Handling
- **SDR10.** All user inputs must be validated and sanitized on both the client and server sides.
- **SDR11.** SQL queries must use parameterized statements or ORM to prevent SQL injection.
- **SDR12.** The application must have proper error handling mechanisms that prevent stack traces or debug information from being exposed to users.

## Secure Development Practices
- **SDR13.** Security requirements must be included from the start of the development lifecycle.
- **SDR14.** All third-party libraries and dependencies must be regularly scanned for vulnerabilities.
- **SDR15.** Static code analysis tools (e.g., SonarQube, Semgrep) must be integrated into the CI pipeline. VER MELHOR!
- **SDR16.** Secrets (e.g., database credentials, API keys) must be stored in secure vaults or environment variables — not hard-coded.

## Logging and Auditing
- **SDR17.** All sensitive operations (e.g., login, role change, file upload, permission updates) must be logged with timestamps and user context.
- **SDR18.** Logs must be securely stored and protected against unauthorized access or tampering.
- **SDR19.** Logs must be reviewed regularly to detect suspicious behavior or policy violations.

## Operating System Interactions
- **SDR20.** Execution of OS-level commands (e.g., directory creation, file write/read) must be sandboxed and validated to prevent command injection.
- **SDR21.** File and directory permissions must be tightly controlled to avoid privilege escalation.
- **SDR22.** Uploaded files must be stored in a secure directory outside of the web root.

## Incident Response & Patching
- **SDR23.** The application must support an audit trail to investigate potential incidents.
- **SDR24.** A security patching process must be in place for addressing discovered vulnerabilities.
- **SDR25.** Critical vulnerabilities must be addressed within 24 hours of detection.

[Back to Documentation](../Documentation.md)
