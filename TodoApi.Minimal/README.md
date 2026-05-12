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

| Profile | URL |
| ------- | --- |
| http | `http://localhost:5258` |
| https | `https://localhost:7171` |

### Explore the API

When running in the **Development** environment, an OpenAPI document is
served at `/openapi/v1.json`.

## Project Structure

```text
TodoApi.Minimal/
├── Program.cs                        # Application entry point and endpoint definitions
├── TodoApi.Minimal.csproj            # Project file with package references
├── appsettings.json                  # Application configuration
├── appsettings.Development.json      # Development-specific configuration
├── TodoApi.Minimal.http              # HTTP request file for testing endpoints
└── Properties/
    └── launchSettings.json           # Launch profiles (ports, environment)
```

## Key Packages

| Package | Purpose |
| ------- | ------- |
| `Microsoft.AspNetCore.OpenApi` | OpenAPI document generation |
| `Microsoft.EntityFrameworkCore` | ORM for data access |
| `Microsoft.EntityFrameworkCore.InMemory` | In-memory database provider for development |
| `Microsoft.EntityFrameworkCore.Sqlite` | SQLite database provider |
| `Microsoft.EntityFrameworkCore.SqlServer` | SQL Server database provider |
| `NSwag.AspNetCore` | Swagger UI for interactive API exploration |

## Tutorial Progress

This project is in its **initial state** — the scaffolded template with a
sample `/weatherforecast` endpoint. The remaining tutorial steps (adding a
model, database context, and CRUD endpoints for Todo items) have not been
completed yet.

## Additional Resources

- [ASP.NET Core Minimal APIs overview](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-10.0)
- [Entity Framework Core documentation](https://learn.microsoft.com/en-us/ef/core/)

