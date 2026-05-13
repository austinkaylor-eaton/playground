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
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for running
  MongoDB via Docker Compose)

### Starting MongoDB with Docker Compose

The project includes a `docker-compose.yml` that runs a MongoDB 8.0 instance and
seeds it with sample book data via `mongo-init.js`.

```bash
docker compose up -d
```

This starts MongoDB on `localhost:27017` and creates the `BookStore` database with a
`Books` collection containing sample data.

#### Useful Docker Compose Commands

| Command | Description |
| ------- | ----------- |
| `docker compose up -d` | Start MongoDB in the background |
| `docker compose down` | Stop and remove the container |
| `docker compose down -v` | Stop, remove container, **and delete data volume** |
| `docker compose logs -f mongodb` | Tail the MongoDB container logs |

#### Connecting to MongoDB Shell

To open an interactive MongoDB shell inside the running container:

```bash
docker exec -it bookstore-mongodb mongosh BookStore
```

Example queries inside the shell:

```javascript
db.Books.find().pretty()
db.Books.countDocuments()
```

### Running the API

```bash
dotnet run
```

The API will be available at the URL specified in `Properties/launchSettings.json`.

## Project Structure

```text
BookStoreApi/
├── Controllers/         # API controllers
├── Properties/          # Launch settings
├── appsettings.json     # Application configuration
├── docker-compose.yml   # MongoDB container definition
├── mongo-init.js        # MongoDB seed data script
├── Program.cs           # Application entry point
└── BookStoreApi.csproj
```

## Tutorial

This project follows the Microsoft Learn tutorial:

- [Create a web API with ASP.NET Core and MongoDB](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-10.0&tabs=visual-studio)

## Solution

[Back to Solution README](../README.md)

