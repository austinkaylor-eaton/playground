---
post_title: "TodoApi - ASP.NET Core Web API"
author1: "austinkaylor-eaton"
post_slug: "todo-api"
microsoft_alias: ""
featured_image: ""
categories: []
tags:
  - aspnet-core
  - web-api
  - entity-framework-core
  - dotnet
ai_note: "AI assisted in the creation of this document."
summary: "A simple Todo Web API built with ASP.NET Core 10 and Entity Framework Core, based on the official Microsoft tutorial."
post_date: "2026-05-12"
---

## Overview

TodoApi is a RESTful Web API for managing todo items, built with ASP.NET Core 10
and Entity Framework Core. The project uses an in-memory database for
development and exposes CRUD endpoints through a controller-based architecture.

This project is based on the official Microsoft tutorial:
[Tutorial: Create a web API with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0&tabs=visual-studio)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

## Project Structure

```
TodoApi/
├── Controllers/
│   └── TodoItemsController.cs   # API controller with CRUD endpoints
├── Database/
│   ├── TodoContext.cs            # EF Core DbContext
│   └── TodoItem.cs               # Database entity
├── Models/
│   └── TodoItemDTO.cs            # Data transfer object
├── Properties/
│   └── launchSettings.json       # Launch profiles
├── appsettings.json              # Application configuration
├── Program.cs                    # Application entry point
├── TodoApi.csproj                # Project file
└── TodoApi.http                  # HTTP request samples
```

## Getting Started

### Run the API

```bash
cd TodoApi
dotnet run
```

By default the API listens on:

- **HTTP:** `http://localhost:5012`
- **HTTPS:** `https://localhost:7170`

### OpenAPI / Swagger UI

In development mode the OpenAPI document is available at `/openapi/v1.json`
and a Swagger UI is served at the root for interactive testing.

## API Endpoints

All endpoints are prefixed with `/api/todoitems`.

| Method   | Route                  | Description              |
| -------- | ---------------------- | ------------------------ |
| `GET`    | `/api/todoitems`       | Get all todo items       |
| `GET`    | `/api/todoitems/{id}`  | Get a todo item by ID    |
| `POST`   | `/api/todoitems`       | Create a new todo item   |
| `PUT`    | `/api/todoitems/{id}`  | Update an existing item  |
| `DELETE` | `/api/todoitems/{id}`  | Delete a todo item       |

### Request / Response Examples

#### Create a todo item

```http
POST /api/todoitems
Content-Type: application/json

{
  "name": "walk dog",
  "isComplete": false
}
```

#### Response

```json
{
  "id": 1,
  "name": "walk dog",
  "isComplete": false
}
```

## Key Dependencies

| Package                                    | Purpose                                        |
| ------------------------------------------ | ---------------------------------------------- |
| `Microsoft.EntityFrameworkCore`             | ORM for data access                            |
| `Microsoft.EntityFrameworkCore.InMemory`    | In-memory database provider for development    |
| `Microsoft.EntityFrameworkCore.Sqlite`      | SQLite database provider                       |
| `Microsoft.EntityFrameworkCore.SqlServer`   | SQL Server database provider                   |
| `Microsoft.AspNetCore.OpenApi`             | OpenAPI document generation                    |
| `NSwag.AspNetCore`                         | Swagger UI middleware                          |

## Architecture Notes

- **DTO pattern** — `TodoItemDTO` decouples the API contract from the internal
  `TodoItem` database entity, preventing over-posting and allowing the schema
  to evolve independently.
- **In-memory database** — The project uses `UseInMemoryDatabase` for quick
  development; swap the provider in `Program.cs` to target SQLite or SQL Server
  for persistent storage.
- **Controller-based API** — Endpoints are defined in `TodoItemsController`
  following standard ASP.NET Core conventions.

## Additional Resources

- [Tutorial: Create a web API with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0&tabs=visual-studio)
- [ASP.NET Core documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core documentation](https://learn.microsoft.com/en-us/ef/core/)

