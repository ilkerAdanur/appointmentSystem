# Appointment System - .NET 9 Microservices

## Overview

This project is a learning-oriented microservices application built with .NET 9.

The goal is to learn and implement modern backend development practices by gradually evolving the architecture into a production-ready microservices system.

The project currently consists of three independent applications:

* ApiGateway
* UserService
* AppointmentService

The services communicate over HTTP through the API Gateway using YARP.

---

# Architecture

```text
                 Client
                    |
                    |
             API Gateway (YARP)
              /              \
             /                \
    UserService        AppointmentService
             |                 |
             |                 |
      SQL Server        SQL Server
       (UserDb)      (AppointmentDb)
             \                 /
              \               /
          Docker Compose Network
```

Each microservice owns its own database and is responsible for its own business logic.

The API Gateway acts as the single entry point for client requests.

---

# Technologies

* .NET 9
* ASP.NET Core Minimal API
* Entity Framework Core
* SQL Server 2022
* Docker
* Docker Compose
* YARP Reverse Proxy
* FluentValidation
* Serilog
* Polly
* Options Pattern
* Dependency Injection
* Health Checks

---

# Project Structure

```text
appointmentSystem
│
├── ApiGateway
│   ├── Dockerfile
│   ├── Program.cs
│   └── appsettings.json
│
├── UserService
│   ├── Configuration
│   ├── Data
│   ├── Dtos
│   ├── ExceptionHandlers
│   ├── Exceptions
│   ├── Filters
│   ├── Migrations
│   ├── Models
│   ├── Services
│   ├── Validators
│   └── Dockerfile
│
├── AppointmentService
│   ├── Configuration
│   ├── Data
│   ├── Dtos
│   ├── ExceptionHandlers
│   ├── Exceptions
│   ├── Filters
│   ├── Migrations
│   ├── Models
│   ├── Services
│   ├── Validators
│   └── Dockerfile
│
├── docker-compose.yml
├── .dockerignore
├── .gitignore
└── appointmentSystem.sln
```

---

# Current Features

## UserService

* User CRUD endpoints
* Entity Framework Core persistence
* SQL Server database integration
* DTO mapping
* Service Layer
* FluentValidation
* Reusable validation endpoint filter
* Global exception handling with `IExceptionHandler`
* Options Pattern configuration
* Serilog logging
* Health check endpoint
* Startup database migrations
* Docker support

---

## AppointmentService

* Appointment CRUD endpoints
* Entity Framework Core persistence
* SQL Server database integration
* DTO mapping
* Service Layer
* FluentValidation
* Reusable validation endpoint filter
* Global exception handling with `IExceptionHandler`
* HTTP communication with UserService
* User existence validation before creating appointments
* Appointment details endpoint combining appointment and user data
* Options Pattern configuration
* Named HttpClient
* Polly retry policy for UserService requests
* Service unavailable handling
* Serilog logging
* Health check endpoint
* Startup database migrations
* Docker support

---

## API Gateway

* Reverse proxy using YARP
* Route forwarding to UserService and AppointmentService
* Single entry point for clients
* Health check endpoint
* Serilog logging
* Docker support

---

# Concepts Implemented

During this project, the following software engineering concepts have been implemented:

* Dependency Injection
* Layered Architecture
* Service Layer Pattern
* DTO Pattern
* Entity Framework Core
* Entity Migrations
* Minimal APIs
* API Gateway
* HTTP Client Factory
* Named HttpClient
* Configuration Management
* Options Pattern
* Validation
* Endpoint Filters
* Global Exception Handling
* Custom Exceptions
* Structured Logging
* Reverse Proxy
* Microservice Communication
* Retry Policies
* Service Availability Handling
* Docker Containers
* Docker Compose
* SQL Server Container
* Health Checks
* Automatic Database Migrations

---

# Logging

Serilog is configured for structured logging.

Current log targets:

* Console output
* Rolling log files

Application logs are written to the `Logs` directory inside each service.

---

# Validation

Input validation is implemented using FluentValidation.

Requests are validated before reaching the business layer through reusable endpoint filters.

Examples of validation rules include:

* Appointment date must be in the future.
* Appointment description cannot be empty.
* User ID must be greater than zero.
* User name cannot be empty.
* User email must be a valid email address.

---

# Exception Handling

Global exception handling is implemented using `IExceptionHandler`.

Custom exceptions are translated into consistent HTTP responses.

Current exception types include:

* `BadRequestException`
* `NotFoundException`
* `ServiceUnavailableException`

Examples:

```text
BadRequestException          -> HTTP 400
NotFoundException            -> HTTP 404
ServiceUnavailableException  -> HTTP 503
Unexpected exception         -> HTTP 500
```

---

# Configuration

Application settings are managed using the Options Pattern instead of accessing `IConfiguration` directly throughout the application.

For example, the AppointmentService reads the UserService address from configuration through a typed configuration class.

This provides:

* Better IntelliSense support
* Stronger configuration structure
* Easier maintenance
* Reduced risk of string key mistakes

---

# API Communication

AppointmentService communicates with UserService through HTTP.

Before a new appointment is created, AppointmentService verifies that the related user exists in UserService.

```text
AppointmentService
        |
        | HTTP GET /users/{id}
        |
        v
UserService
```

This demonstrates synchronous service-to-service communication.

Polly is used to retry transient failures when AppointmentService calls UserService.

If UserService is unavailable after retry attempts, AppointmentService returns a `503 Service Unavailable` response.

---

# Database

Each microservice owns its own SQL Server database.

```text
UserService        -> UserDb
AppointmentService -> AppointmentDb
```

The databases run inside a SQL Server Docker container.

There are no database-level foreign keys between services.

For example, `Appointment.UserId` is not a SQL foreign key to the Users table because UserService and AppointmentService own separate databases.

Relationships between services are validated through service-to-service communication.

Entity Framework Core migrations are automatically applied during service startup.

---

# Docker

Each application includes its own Dockerfile.

The complete application environment is managed through Docker Compose.

Docker Compose starts:

* SQL Server
* UserService
* AppointmentService
* API Gateway

The SQL Server container uses a persistent Docker volume:

```text
sqlserver_data
```

This keeps database data available even when containers are restarted.

---

# Health Checks

Each service exposes a health check endpoint:

```text
UserService:        /health
AppointmentService: /health
ApiGateway:         /health
```

Docker Compose waits until SQL Server is healthy before starting dependent services.

This prevents services from attempting database connections before SQL Server is ready.

---

# Running the Project

## Prerequisites

* .NET 9 SDK
* Docker Desktop
* Docker Compose

## Start the Complete Environment

From the root folder of the repository:

```bash
docker compose up --build
```

Docker Compose will automatically:

1. Start SQL Server.
2. Wait for SQL Server to become healthy.
3. Start UserService.
4. Start AppointmentService.
5. Start ApiGateway.
6. Apply Entity Framework Core migrations during service startup.

## Stop the Environment

```bash
docker compose down
```

## Stop and Remove Database Data

```bash
docker compose down -v
```

The `-v` option removes the SQL Server Docker volume and deletes the container database data.

---

# Service Endpoints

## API Gateway

```text
http://localhost:8080
```

## UserService

```text
http://localhost:8081
```

## AppointmentService

```text
http://localhost:8082
```

## Health Checks

```text
http://localhost:8080/health
http://localhost:8081/health
http://localhost:8082/health
```

---

# Database Connection for DBeaver or SSMS

The SQL Server container is exposed on port `14330`.

Use the following connection values:

```text
Host: localhost
Port: 14330
Database: master
Username: sa
Password: Software2026resdvxcvd!
```

After migrations are applied, the following databases should be visible:

```text
UserDb
AppointmentDb
```

---

# Current Roadmap

## Completed

* Project setup
* Git and GitHub
* Solution structure
* UserService
* AppointmentService
* API Gateway
* Entity Framework Core
* SQL Server LocalDB
* Entity Migrations
* DTOs
* Service Layer
* Dependency Injection
* FluentValidation
* Validation Filters
* Global Exception Handling
* Options Pattern
* Serilog
* HTTP Client Factory
* Named HttpClient
* Polly Retry Policy
* Service Unavailable Handling
* Health Checks
* Dockerfiles
* Docker Compose
* SQL Server Container
* Docker Health Checks
* Automatic EF Core Migrations

## Planned

* JWT Authentication
* Refresh Tokens
* RabbitMQ
* Redis Cache
* gRPC
* OpenTelemetry
* Distributed Tracing
* Rate Limiting
* Controller-Based APIs
* CQRS
* MediatR
* Configuration and Secrets Management

---

# Purpose

This repository is intended as a learning project focused on modern .NET backend development and microservice architecture.

The goal is not only to build a working application, but also to understand the reasoning behind architectural decisions and industry practices.
