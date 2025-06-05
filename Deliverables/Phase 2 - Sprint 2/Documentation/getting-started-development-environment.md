# Library Online Rental System - Development Environment Setup

# 1. GIT Setup

## GIT Installation:

Follow this tutorial: https://git-scm.com/downloads

## GIT Configuration

   `git config –global user.name = “Name Surname”
   git config –global user.name = studentnumber@isep.ipp.pt
   git config --global core.editor "nano"`

GUI: https://git-scm.com/downloads/guis


## Configuring SSH Key on Github

Execute the following commands:

   ssh-keygen

After, register the generated key on github:

https://docs.github.com/en/authentication/connecting-to-github-with-ssh/generating-a-newssh-key-and-adding-it-to-the-ssh-agent


# Documentation

## Tools

- **Markdown Editors**
  - [Visual Studio Code](https://code.visualstudio.com/) with [Markdown All in One](https://marketplace.visualstudio.com/items?itemName=yzhang.markdown-all-in-one)
  - [IntelliJ IDEA](https://www.jetbrains.com/idea/)

## API Documentation

- **Swagger UI**
  - Accessible at `/swagger` when running the application
  - Provides interactive API documentation
  - Test endpoints directly from the browser

## Architecture Documentation

- **PlantUML** for diagrams
  - [VS Code Extension](https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml)
  - [Online Editor](https://plantuml.com/)

## Code Snippets

For consistent code screenshots:
- [Carbon](https://carbon.now.sh/)
- [Codeimg.io](https://codeimg.io/)

# Development Tools

## Recommended IDEs

- **Rider**
  - [Download](https://www.jetbrains.com/rider/)
  - Best for .NET development with great C# support
  - Integrated database tools

- **Visual Studio 2022**
  - [Download Community Edition](https://visualstudio.microsoft.com/vs/community/)
  - Install with "ASP.NET and web development" workload
  - Includes SQL Server Data Tools

# System Administration

## Remote Access to the servers:

- [Download](https://mobaxterm.mobatek.net/download.html)

## Database Tools

- **MySQL Workbench**
  - [Download](https://www.mysql.com/products/workbench/)
  - Visual database design and management
  - SQL development and administration

## API Testing

- **Postman**
  - [Download](https://www.postman.com/downloads/)
  - API testing and documentation
  - Collection runner for automated testing

## Containerization

- **Docker Desktop**
  - [Download](https://www.docker.com/products/docker-desktop/)
  - Required for running the database in a container
  - Useful for consistent development environments
