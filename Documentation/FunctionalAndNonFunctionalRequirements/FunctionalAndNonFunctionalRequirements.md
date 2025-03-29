# Functional and Non-Functional Requirements

## Functional Requirements (FR)

> These define the core features and operations the system must provide.

### User

- **FR1.** The user shall be able to register and authenticate within the application.
- **FR2.** The user shall be able to view the list of available books.
- **FR3.** The user shall be able to search for books by title, author, publisher, or category.
- **FR4.** The user shall be able to view detailed information about a specific book.
- **FR5.** The user shall be able to rent a book if copies are available.
- **FR6.** The user shall be able to view their personal rental history.
- **FR7.** The user shall be able to rate books they have previously rented.
- **FR8.** The user shall be able to suggest new books for the library to acquire.

### Library Manager

- **FR9.** The manager shall be able to add, update, or remove books from the catalogue.
- **FR10.** The manager shall be able to view and manage active rentals.
- **FR11.** The manager shall be able to approve or reject book suggestions from users.

### Administrator

- **FR12.** The administrator shall be able to create, edit, or remove user accounts.
- **FR13.** The administrator shall be able to assign roles to users (user, manager, admin).
- **FR14.** The administrator shall be able to view audit logs and system activity.

---

## Non-Functional Requirements (NFR)

> These define quality attributes, constraints, and system properties.

### Security

- **NFR1.** All communication between client and server must be encrypted (HTTPS).
- **NFR2.** User passwords must be securely hashed (e.g., using bcrypt).
- **NFR3.** The system must enforce role-based access control (RBAC).
- **NFR4.** The API must be protected against common threats (e.g., SQL Injection, XSS, CSRF).
- **NFR5.** All sensitive actions must be logged for auditing purposes.

### Performance & Scalability

- **NFR6.** The API should respond in under 500ms for 95% of requests.
- **NFR7.** The system should support at least 100 concurrent users without performance degradation.
- **NFR8.** The database should be horizontally scalable for read operations.

### Maintainability & Deployment

- **NFR9.** The codebase must follow clean architecture principles with clear separation of concerns.
- **NFR10.** The system must support CI/CD pipelines with automated testing.
- **NFR11.** The deployment should be containerised (e.g., Docker) and support multiple environments (dev, staging, prod).

### Usability & Availability

- **NFR12.** The API must follow RESTful conventions and return meaningful error messages.
- **NFR13.** The system must maintain 99% uptime availability (internal SLA).
- **NFR14.** API documentation must be accessible through Swagger/OpenAPI.

[Back to Documentation](~/Documentation/Documentation.md)