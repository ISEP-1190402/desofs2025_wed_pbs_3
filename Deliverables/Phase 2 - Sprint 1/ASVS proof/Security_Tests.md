# 🔐 Security Tests – Entity Property Restrictions

This table outlines the security validations enforced for each business entity in the Library Online Rental System.

| **Entity**     | **Property**          | **Security Restriction / Validation**                                                                                          |
|----------------|-----------------------|-------------------------------------------------------------------------------------------------------------------------------|
| **User**       | Email                 | Must be validated upon registration via confirmation email. [SDR4]                                                             |
|                | Password              | Must be at least 8 characters, include uppercase, lowercase, number, and special character. [SDR3]                            |
|                | MFA Email             | Supports Multi-Factor Authentication via email code. [SDR1]                                                                    |
|                | Role                  | Assigned by admin. Access controlled using RBAC with least privilege. [SDR5, FR14]                                             |
| **Book**       | General Metadata      | Titles, authors, categories, etc., are managed by the Library Manager only. [FR10]                                             |
| **Rental**     | BookId, UserId        | Users can only rent if copies are available; can't rent on behalf of other users. [FR6, User Stories]                          |

> All data is protected in transit using HTTPS/TLS [NFR1], and passwords are hashed with bcrypt. [NFR2, SDR8]  
> All user-submitted data must be validated server-side. [SDR10]  
> Logging is applied to sensitive actions and sent to an external logging system. [SDR12]                                                                          |

There are security tests that verify and validate these constraints for entities implemented at this stage.
