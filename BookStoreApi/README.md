---
post_title: BookStore API
author1: austinkaylor-eaton
post_slug: bookstore-api
microsoft_alias: austinkaylor-eaton
featured_image: ''
categories: []
tags: [aspnetcore, mongodb, webapi]
ai_note: AI assisted in the creation of this document.
summary: A RESTful Web API for managing books, built with ASP.NET Core 10 and MongoDB.
post_date: 2026-05-13
---

## BookStore API

A RESTful Web API for managing books in a bookstore, built with ASP.NET Core 10 and MongoDB.

## Overview

This project demonstrates how to build a web API using ASP.NET Core with MongoDB as the
data store. It follows the controller-based API pattern and provides CRUD operations for
managing book records.

## Technologies

| Technology | Purpose |
| ---------- | ------- |
| ASP.NET Core 10 | Web framework |
| MongoDB | NoSQL document database |
| OpenAPI | API documentation |

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [MongoDB](https://www.mongodb.com/try/download/community) running locally or a
  connection string to a remote instance

### Running the API

```bash
dotnet run
```

The API will be available at the URL specified in `Properties/launchSettings.json`.

## Project Structure

```text
BookStoreApi/
├── Controllers/       # API controllers
├── Properties/        # Launch settings
├── appsettings.json   # Application configuration
├── Program.cs         # Application entry point
└── BookStoreApi.csproj
```

## Tutorial

This project follows the Microsoft Learn tutorial:

- [Create a web API with ASP.NET Core and MongoDB](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-10.0&tabs=visual-studio)

## Solution

[Back to Solution README](../README.md)

