# Appointment System - .NET 9 Microservices

## Overview

This project is a learning-oriented microservices application built with .NET 9. The goal is to learn and implement modern backend development practices by gradually evolving the architecture into a production-ready microservices system.

The project currently consists of three independent applications:

* ApiGateway
* UserService
* AppointmentService

The services communicate over HTTP through the API Gateway using YARP.

---

# Architecture

```
                 Client
                    |
                    |
             API Gateway (YARP)
              /              \
             /                \
    UserService        AppointmentService
            \               /
             \             /
             SQL Server (LocalDB)
```

Each microservice owns its own database and is responsible for its own business logic.

---

# Technologies

* .NET 9
* ASP.NET Core Minimal API
* Entity Framework Core
* SQL Server LocalDB
* YARP Reverse Proxy
* FluentValidation
* Serilog
* Options Pattern
* Dependency Injection

---

# Project Structure

```
appointmentSystem
│
├── ApiGateway
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
│   └── Validators
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
│   └── Validators
│
└── appointmentSystem.sln
```

---

# Current Features

## UserService

* CRUD endpoints
* Entity Framework Core
* SQL Server LocalDB
* DTO mapping
* Service Layer
* FluentValidation
* Global Exception Handling
* Options Pattern
* Serilog logging

---

## AppointmentService

* CRUD endpoints
* Entity Framework Core
* SQL Server LocalDB
* DTO mapping
* Service Layer
* FluentValidation
* Global Exception Handling
* HTTP communication with UserService
* User existence validation before creating appointments
* Appointment details endpoint combining data from multiple services
* Options Pattern
* Serilog logging

---

## API Gateway

* Reverse Proxy using YARP
* Route forwarding
* Single entry point for clients

---

# Concepts Implemented

During this project the following software engineering concepts have been implemented:

* Dependency Injection
* Layered Architecture
* Service Layer Pattern
* DTO Pattern
* Entity Framework Core
* Entity Migrations
* Minimal APIs
* API Gateway
* HTTP Client Factory
* Configuration Management
* Options Pattern
* Validation
* Global Exception Handling
* Structured Logging
* Reverse Proxy
* Microservice Communication

---

# Logging

Serilog has been configured for structured logging.

Current log targets:

* Console
* Rolling file logs

---

# Validation

Input validation is implemented using FluentValidation.

Requests are validated before reaching the business layer by using reusable endpoint filters.

---

# Exception Handling

Global exception handling is implemented using `IExceptionHandler`.

Custom exceptions are translated into consistent HTTP responses.

---

# Configuration

Application settings are managed using the Options Pattern instead of directly accessing `IConfiguration`.

---

# API Communication

AppointmentService validates users by sending HTTP requests to UserService before creating appointments.

This demonstrates synchronous communication between microservices.

---

# Database

Each service owns its own database.

There are no database-level foreign keys between services.

Relationships are validated through service-to-service communication.

---

# Current Roadmap

Completed

* Project setup
* Git & GitHub
* Solution structure
* UserService
* AppointmentService
* API Gateway
* Entity Framework Core
* SQL Server LocalDB
* Migrations
* DTOs
* Service Layer
* FluentValidation
* Validation Filters
* Global Exception Handling
* Options Pattern
* Serilog
* HTTP Client Factory

Planned

* Polly
* Health Checks
* Docker
* Docker Compose
* JWT Authentication
* Refresh Tokens
* RabbitMQ
* Redis
* gRPC
* OpenTelemetry
* Controller-based APIs
* CQRS
* MediatR

---

# Running the Project

Start the services in the following order:

1. UserService
2. AppointmentService
3. ApiGateway

The API Gateway acts as the single entry point for all client requests.

---

# Purpose

This repository is intended as a learning project focused on modern .NET backend development and microservice architecture.

The goal is not only to build a working application but also to understand the reasoning behind architectural decisions and industry practices.
