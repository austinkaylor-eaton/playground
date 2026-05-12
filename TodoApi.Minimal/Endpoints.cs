using Microsoft.EntityFrameworkCore;
using Constants = TodoApi.Minimal.Constants;

namespace TodoApi.Minimal;

/// <summary>
/// Contains methods for registering and interacting with the API endpoints,
/// such as the URL paths and HTTP methods for each endpoint,
/// as well as any additional metadata that may be useful for documentation or testing purposes
/// </summary>
public static class Endpoints
{
    /// <summary>
    /// Registers the endpoints for the <see cref="TodoItem"/>s resource, such as the URL paths and HTTP methods for each endpoint,
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance to register the endpoints with.</param>
    public static void RegisterTodoItemsEndpoints(this WebApplication app)
    {
        var todoItemEndpoints = app.MapGroup($"/{Constants.TodoItems.Tag.ToLowerInvariant()}");

        todoItemEndpoints.MapGet("/", async (TodoDb db) =>
                await db.Todos.ToListAsync())
            .WithName("GetAllTodos")
            .WithSummary($"Get all {nameof(TodoItem)}s")
            .WithDescription($"Retrieves the complete list of {nameof(TodoItem)}s from the database.")
            .WithTags(Constants.TodoItems.Tag)
            .Produces<List<TodoItem>>();

        todoItemEndpoints.MapGet("/complete", async (TodoDb db) =>
                await db.Todos.Where(t => t.IsComplete).ToListAsync())
            .WithName("GetCompleteTodos")
            .WithSummary($"Get completed {nameof(TodoItem)}s")
            .WithDescription($"Retrieves only {nameof(TodoItem)}s that have been marked as complete.")
            .WithTags(Constants.TodoItems.Tag)
            .Produces<List<TodoItem>>();

        todoItemEndpoints.MapGet("/{id:int}", async (int id, TodoDb db) =>
                await db.Todos.FindAsync(id)
                    is { } todo
                    ? Results.Ok(todo)
                    : Results.NotFound())
            .WithName("GetTodoById")
            .WithSummary($"Get a {nameof(TodoItem)} by ID")
            .WithDescription($"Retrieves a single {nameof(TodoItem)} matching the specified ID. Returns 404 if not found.")
            .WithTags(Constants.TodoItems.Tag)
            .Produces<TodoItem>()
            .Produces(StatusCodes.Status404NotFound);

        todoItemEndpoints.MapPost("/", async (TodoItem todo, TodoDb db) =>
            {
                db.Todos.Add(todo);
                await db.SaveChangesAsync();

                return Results.Created($"/{Constants.TodoItems.Tag.ToLowerInvariant()}/{todo.Id}", todo);
            })
            .WithName("CreateTodo")
            .WithSummary($"Create a new {nameof(TodoItem)}")
            .WithDescription($"Adds a new {nameof(TodoItem)} to the database and returns the created item with its assigned ID.")
            .WithTags(Constants.TodoItems.Tag)
            .Produces<TodoItem>(StatusCodes.Status201Created);

        todoItemEndpoints.MapPut("/{id:int}", async (int id, TodoItem inputTodo, TodoDb db) =>
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo is null) return Results.NotFound();

                todo.Name = inputTodo.Name;
                todo.IsComplete = inputTodo.IsComplete;

                await db.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("UpdateTodo")
            .WithSummary($"Update an existing {nameof(TodoItem)}")
            .WithDescription($"Updates the name and completion status of a {nameof(TodoItem)} matching the specified ID. Returns 404 if not found.")
            .WithTags(Constants.TodoItems.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        todoItemEndpoints.MapPatch("/{id:int}", async (int id, TodoItemPatchDTO inputTodo, TodoDb db) =>
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo is null) return Results.NotFound();

                if (inputTodo.Name is not null) todo.Name = inputTodo.Name;
                if (inputTodo.IsComplete is not null) todo.IsComplete = inputTodo.IsComplete.Value;

                await db.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("PatchTodo")
            .WithSummary($"Patch a {nameof(TodoItem)}")
            .WithDescription($"Updates specific fields of a {nameof(TodoItem)} matching the specified ID. Returns 404 if not found.")
            .WithTags(Constants.TodoItems.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        todoItemEndpoints.MapDelete("/{id:int}", async (int id, TodoDb db) =>
            {
                if (await db.Todos.FindAsync(id) is not { } todo)
                {
                    return Results.NotFound();
                }

                db.Todos.Remove(todo);
                await db.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("DeleteTodo")
            .WithSummary($"Delete a {nameof(TodoItem)}")
            .WithDescription($"Removes a {nameof(TodoItem)} matching the specified ID from the database. Returns 404 if not found.")
            .WithTags(Constants.TodoItems.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}