# Functional and Non-Functional Requirements

## Functional Requirements (FR)

> These define the core features and operations the system must provide.

### User

- **FR1.** The user shall be able to register and authenticate within the application.
- **FR2.** The user shall be able to change their profile anytime (password and MFA-associated email address). When the MFA email is changed, a notification must be sent to the registered email address associated with the account to alert the user of this change.
- **FR3.** The user shall be able to view the list of available books.
- **FR4.** The user shall be able to search for books by title, author, publisher, or category.
- **FR5.** The user shall be able to view detailed information about a specific book.
- **FR6.** The user shall be able to rent a book if copies are available.
- **FR7.** The user shall be able to view their personal rental history.
- **FR8.** The user shall be able to rate books they have previously rented.
- **FR9.** The user shall be able to suggest new books for the library to acquire.

### Library Manager

- **FR10.** The manager shall be able to add, update, or remove books from the catalogue.
- **FR11.** The manager shall be able to view and manage active rentals.
- **FR12.** The manager shall be able to approve or reject book suggestions from users.

### Administrator

- **FR13.** The administrator shall be able to create, edit, or remove user accounts.
- **FR14.** The administrator shall be able to assign roles to users (user, manager, admin).
- **FR15.** The administrator shall be able to view audit logs and system activity.

---

## Non-Functional Requirements (NFR)

> These define quality attributes, constraints, and system properties.

### Security

- **NFR1.** All communication between client and server must be encrypted (HTTPS).
- **NFR2.** User passwords must be securely hashed (e.g., using bcrypt).
- **NFR3.** The system must enforce role-based access control (RBAC).
- **NFR4.** The API must be protected against common threats (e.g., SQL Injection, XSS, CSRF).
- **NFR5.** All sensitive actions must be logged for auditing purposes.
- NFR6. All users must authenticate using two-factor authentication (code via email).

### Performance & Scalability

- **NFR6.** The API should respond in under 500ms for 95% of requests.
- **NFR7.** The system should support at least 100 concurrent users without performance degradation.
- **NFR8.** The database should be horizontally scalable for read operations.

### Maintainability & Deployment

- **NFR9.** The codebase must follow clean architecture principles with clear separation of concerns.
- **NFR10.** The system must support CI/CD pipelines with automated testing.
- **NFR11.** The deployment should be containerised (e.g., Docker) and support multiple environments (dev, staging, prod).
- **NFR12.** At least two backup copies must be stored in separate online locations.
- **NFR13.** At least one copy of the backup must be stored offline.


### Usability & Availability

- **NFR12.** The API must follow RESTful conventions and return meaningful error messages.
- **NFR13.** The system must maintain 99% uptime availability (internal SLA).
- **NFR14.** API documentation must be accessible through Swagger/OpenAPI.

[Back to Documentation](../Documentation.md)
