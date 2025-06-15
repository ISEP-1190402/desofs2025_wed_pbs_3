# Library Online Rental System - Features

## Table of Contents

1. [Core Features](#1-core-features)  
2. [Authentication & Authorization](#2-authentication--authorization)  
3. [User Management](#3-user-management)  
4. [Book Management](#4-book-management)  
5. [Future Features](#5-future-features)

---

## . Core Features

### 1.1 Secure Authentication
- JWT (JSON Web Tokens) authentication
- Keycloak integration for identity management
- Protection against XSS attacks

### 1.2 Clean Architecture
- Rich domain model with business validations
- Dependency

## 2. Authentication & Authorization

### 2.1 Supported Flows
- User registration
- Login/Logout
- Password recovery
<!--- Token refresh -->


### 2.2 Access Levels
- **Administrator**: Full system access
- **Library Manager**: Book and request management
- **User**: Book rental
- **Guest**: Read-only access

## 3. User Management

### 3.1 User Profile
- Email-validated registration
- Profile updates
- Rental history

### 3.2 Administration
- User listing
- Account creation/deactivation
- Role assignment
- Permission management

## 4. Book Management

### 4.1 Catalog
- Available books listing
- Search by title, author, ISBN
<!-- - Advanced filters -->
- Complete book details

### 4.2 Rental
- Book requests
- Rental history
<!-- - Rental renewals -->

## 5. Future Features
Add a book recommendation feature based on rental history
Add automated reminders for rental expiration
Add automated monthly subscription of book rental within a category
