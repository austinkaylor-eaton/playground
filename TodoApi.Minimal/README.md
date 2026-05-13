---
post_title: "TodoApi.Minimal — Minimal Web API"
author1: "Austin Kaylor"
post_slug: "todoapi-minimal"
microsoft_alias: "N/A"
featured_image: ""
categories: []
tags:
  - aspnetcore
  - minimal-api
  - dotnet
  - tutorial
ai_note: "AI-assisted content creation"
summary: "A minimal web API project built by following the official ASP.NET Core Minimal API tutorial targeting .NET 10."
post_date: "2026-05-12"
---

⬅️ [Back to Solution](../README.md)

## Overview

This project is a minimal web API built with ASP.NET Core 10 and the
Minimal API approach. It was created by following the official Microsoft
tutorial:

[Tutorial: Create a minimal API with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-10.0&tabs=visual-studio-code)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Getting Started

### Run the Application

```bash
cd TodoApi.Minimal
dotnet run
```

The API is available at the URLs configured in
`Properties/launchSettings.json`:

| Profile | URL                      |
|---------|--------------------------|
| http    | `http://localhost:5258`  |
| https   | `https://localhost:7171` |

### Explore the API

When running in the **Development** environment, Swagger UI is available
at `/swagger` for interactive API exploration. The OpenAPI document is
served at `/swagger/TodoApi/swagger.json`.

## API Endpoints

All endpoints are grouped under `/todoitems`.

| Method | Route                 | Description                        |
|--------|-----------------------|------------------------------------|
| GET    | `/todoitems`          | Retrieve all to-do items           |
| GET    | `/todoitems/{id}`     | Retrieve a to-do item by ID        |
| GET    | `/todoitems/complete` | Retrieve all completed to-do items |
| POST   | `/todoitems`          | Create a new to-do item            |
| PUT    | `/todoitems/{id}`     | Update an existing to-do item      |
| PATCH  | `/todoitems/{id}`     | Partially update a to-do item      |
| DELETE | `/todoitems/{id}`     | Delete a to-do item                |

## Project Structure

```text
TodoApi.Minimal/
├── Program.cs               # Application entry point and middleware configuration
├── ApiEndpoints.cs          # Endpoint definitions using RouteGroupBuilder
├── TodoItem.cs              # Database entity model
├── TodoItemDTO.cs           # Data transfer object for full CRUD operations
├── TodoItemPatchDTO.cs      # Data transfer object for partial updates
├── TodoDb.cs                # EF Core DbContext
├── Constants.cs             # Shared constants (endpoint tags, URL paths)
├── TodoApi.Minimal.csproj   # Project file with package references
├── TodoApi.Minimal.http     # HTTP request file for testing endpoints
├── appsettings.json         # Application configuration
├── appsettings.Development.json
└── Properties/
    └── launchSettings.json  # Launch profiles (ports, environment)
```

## Key Packages

| Package                                   | Purpose                                     |
|-------------------------------------------|---------------------------------------------|
| `Microsoft.AspNetCore.OpenApi`            | OpenAPI document generation                 |
| `Microsoft.EntityFrameworkCore`           | ORM for data access                         |
| `Microsoft.EntityFrameworkCore.InMemory`  | In-memory database provider for development |
| `Microsoft.EntityFrameworkCore.Sqlite`    | SQLite database provider                    |
| `Microsoft.EntityFrameworkCore.SqlServer` | SQL Server database provider                |
| `NSwag.AspNetCore`                        | Swagger UI for interactive API exploration  |

## Tutorial Progress

This project is **complete** — all tutorial steps have been implemented,
including the model, database context, CRUD endpoints, DTOs to prevent
over-posting, and Swagger UI for interactive API exploration. A PATCH
endpoint was added as an extension beyond the original tutorial.

## Additional Resources

- [ASP.NET Core Minimal APIs overview](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-10.0)
- [Entity Framework Core documentation](https://learn.microsoft.com/en-us/ef/core/)
- [Typed Results vs Results](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-10.0#typedresults-vs-results)

