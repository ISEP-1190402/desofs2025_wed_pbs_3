# Abuse Cases

This section outlines potential misuse or attack scenarios that may target the system. Each abuse case is described with the actor, the threat, and the impacted assets. These are important to identify risks early and design appropriate security countermeasures.

## UC01 — Brute Force Authentication

- **Actor:** Malicious User
- **Description:** Attacker repeatedly tries different username/password combinations to gain unauthorized access.
- **Assets Affected:** User accounts, authentication system.
- **Impact:** Account compromise, potential data breach.
- **Mitigation:**
  - Implement rate limiting and account lockouts after consecutive failed attempts.
  - Enforce MFA.

---

## UC02 — Role Escalation via API Manipulation

- **Actor:** Authenticated User  
- **Description:** A regular user attempts to escalate their role (e.g., to Manager or Admin) by modifying API requests such as changing the JSON body or using direct role-assignment endpoints.  
- **Assets Affected:** Authorization system, administrative functionalities  
- **Impact:** Unauthorized access to critical features like user management, rental operations, audit logs  
- **Mitigation:**  
  - Enforce strict Role-Based Access Control (RBAC) at the API level  
  - Ensure all role assignments and changes are performed only by authorized users (e.g., Admins), with validation at the backend  
  - Never expose any endpoints that allow self-role modification or privilege elevation  
  - Log all role change attempts, including denied ones, for auditing

---

## UC03 — SQL Injection

- **Actor:** External Attacker
- **Description:** Malicious input is used in a database query to extract or manipulate sensitive data.
- **Assets Affected:** Database integrity, user data.
- **Impact:** Data leakage, unauthorized data modification, full DB compromise.
- **Mitigation:**
  - Use parameterized queries or ORM.
  - Sanitize and validate all inputs.

---

## UC04 — File Upload Exploitation

- **Actor:** Authenticated User
- **Description:** Attacker uploads a malicious file disguised as a book cover or attachment.
- **Assets Affected:** File system, web server.
- **Impact:** Remote code execution, denial of service, system compromise.
- **Mitigation:**
  - Restrict allowed file types and size.
  - Store files outside the web root.
  - Validate file headers, not just extensions.

---

## UC05 — Directory Traversal

- **Actor:** Authenticated or Anonymous User
- **Description:** Attacker provides a file path that navigates outside the intended directory.
- **Assets Affected:** File system, private server files.
- **Impact:** Unauthorized file access (e.g., `/etc/passwd`).
- **Mitigation:**
  - Normalize and validate all path inputs.
  - Restrict file access to allowed directories only.

---

## UC06 — Session Hijacking

- **Actor:** Network Attacker
- **Description:** Attacker intercepts or reuses a session token.
- **Assets Affected:** User session, identity, data.
- **Impact:** Unauthorized access to user accounts.
- **Mitigation:**
  - Use HTTPS for all communication.
  - Regenerate session tokens on login/logout.
  - Implement token expiration and revocation.

---

## UC07 — Unauthorized OS Command Execution

- **Actor:** Authenticated User  
- **Description:** The attacker injects shell characters or malicious payloads into parameters (e.g., file names or paths), which are then used in insecure OS command executions via the backend.  
- **Assets Affected:** Server, filesystem, system processes  
- **Impact:** Arbitrary command execution, server compromise, potential full system takeover  
- **Mitigation:**  
  - Use secure .NET APIs such as `System.Diagnostics.Process` with argument escaping (e.g., `ProcessStartInfo.ArgumentList`)  
  - Avoid using `cmd.exe` or shell invocation unless absolutely necessary  
  - Validate and sanitize all inputs used in file operations or passed to system processes  
  - Execute backend processes with the least privileged system account  
  - Isolate the application in a sandboxed environment (e.g., Docker container with limited OS access)  
  - Implement extensive logging and monitoring of all system-level interactions

---

## UC08 — Log Forging or Injection

- **Actor:** Authenticated User
- **Description:** Malicious user includes newline or escape characters in input to forge logs.
- **Assets Affected:** Log integrity, audit trail.
- **Impact:** Log poisoning, tampering, confusion in forensic analysis.
- **Mitigation:**
  - Sanitize log entries.
  - Log in structured formats like JSON.

---

## UC09 — Denial of Service (DoS)

- **Actor:** External Attacker or Abusive User
- **Description:** Repeated requests to expensive endpoints (e.g., book search, upload) to exhaust system resources.
- **Assets Affected:** API availability, server performance.
- **Impact:** Service unavailability for legitimate users.
- **Mitigation:**
  - Rate limiting.
  - Throttling and queuing strategies.
  - Protect endpoints using API gateway rules or WAF.

---

## UC10 — Unauthorized Data Access via Broken Access Control

- **Actor:** Authenticated User
- **Description:** A user tries to access another user’s data or rental history via URL manipulation.
- **Assets Affected:** User data confidentiality.
- **Impact:** Privacy violations, trust issues.
- **Mitigation:**
  - Enforce strict ownership checks at the backend.
  - Never trust client-side IDs or filters alone.

