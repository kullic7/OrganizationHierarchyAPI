# OrganizationHierarchyAPI

REST API for managing organizational structures of companies and their employees.

---

# Technologies

* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* Microsoft SQL Server
* Docker & Docker Compose
* Scalar
* TeaPie

---

---

# Requirements

Installed software:

* Docker Desktop
* .NET SDK 10
* TeaPie CLI (for API tests)
* SQL Server Management Studio

---

# Environment Variables

Create `.env` file in the root directory:

```env
SA_PASSWORD=Your_password123!
DB_NAME=OrganizationDb
DB_PORT=1433
API_PORT=5000
```

---

# Running the Project

Run the application from the root directory:

```bash
docker compose up --build
```

This command automatically:

* builds the API container
* starts SQL Server container
* creates the database
* applies Entity Framework Core migrations
* seeds demo data

---

# API Documentation

Base URL:

```text
http://localhost:5000
```

Scalar API documentation:

```text
http://localhost:5000/scalar
```

---

---

# Running TeaPie Tests

## Install TeaPie

```bash
dotnet tool install -g TeaPie.Tool
```

---

## Run All Tests

From the root directory:

```bash
teapie test ./Tests
```

---

## Run Single Test File

```bash
teapie test ./Tests/001-Employees.http
```

---

# Available Tests

```text
Tests/
├── 001-Employees.http
├── 002-Companies.http
└── 003-OrganizationUnits.http
```

Tests cover:

* GET endpoints
* POST endpoints
* PUT endpoints
* DELETE endpoints
* validation errors
* hierarchy validation
* manager validation
* invalid requests

---

# Organizational Hierarchy Rules

The application supports a maximum 4-level hierarchy:

```text
Company
└── Division
    └── Project
        └── Department
```

Validation rules:

* Company cannot have parent
* Division must have parent of type Company
* Project must have parent of type Division
* Department must have parent of type Project
* Manager must belong to the same company
* Organization unit code must be unique inside company
* Organization unit with children cannot be deleted
* Employee managing organization units cannot be deleted
* Company with employees or organization units cannot be deleted

---

# Seed Data

At startup the database is automatically populated with demo data:

* 3 companies
* divisions
* projects
* departments
* employees
* managers

---

# SQL Script

Generated SQL script:

```text
Scripts/init.sql
```

The script can be generated using:

```bash
dotnet ef migrations script -o ../Scripts/init.sql
```

---

# Stopping the Application

```bash
docker compose down
```

To remove volumes and database:

```bash
docker compose down -v
```

---
