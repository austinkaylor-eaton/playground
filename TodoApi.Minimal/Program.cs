using System.Globalization;
using Microsoft.EntityFrameworkCore;
using TodoApi.Minimal;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

#region Swagger Middleware

/*
 * Enables the API Explorer, which is a service that provides metadata about the HTTP API.
 * The API Explorer is used by Swagger to generate the Swagger document.
 */
builder.Services.AddEndpointsApiExplorer();
/*
 * Adds the Swagger OpenAPI document generator to the application services and
 * configures it to provide more information about the API, such as its title and version
 *
 * See https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-8.0&viewFallbackFrom=aspnetcore-10.0&tabs=visual-studio#customize-api-documentation
 * for more information
 */
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "TodoAPI";
    config.Title = "TodoAPI v1";
    config.Version = "v1";
});

#endregion

var app = builder.Build();

#region Swagger Middleware

/*
 * Enables the Swagger UI middleware, which provides a web-based interface for exploring and testing the API.
 * The Swagger UI is automatically generated based on the OpenAPI document.
 */
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "TodoAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

#endregion

#pragma warning disable CA2007
#pragma warning disable CA1308
var todoItemEndpoints = app.MapGroup($"/{Constants.TodoItemsTag.ToLowerInvariant()}");

todoItemEndpoints.MapGet("/", async (TodoDb db) =>
        await db.Todos.ToListAsync())
    .WithName("GetAllTodos")
    .WithSummary($"Get all {nameof(TodoItem)}s")
    .WithDescription($"Retrieves the complete list of {nameof(TodoItem)}s from the database.")
    .WithTags(Constants.TodoItemsTag)
    .Produces<List<TodoItem>>();

todoItemEndpoints.MapGet("/complete", async (TodoDb db) =>
        await db.Todos.Where(t => t.IsComplete).ToListAsync())
    .WithName("GetCompleteTodos")
    .WithSummary($"Get completed {nameof(TodoItem)}s")
    .WithDescription($"Retrieves only {nameof(TodoItem)}s that have been marked as complete.")
    .WithTags(Constants.TodoItemsTag)
    .Produces<List<TodoItem>>();

todoItemEndpoints.MapGet("/{id:int}", async (int id, TodoDb db) =>
        await db.Todos.FindAsync(id)
            is { } todo
            ? Results.Ok(todo)
            : Results.NotFound())
    .WithName("GetTodoById")
    .WithSummary($"Get a {nameof(TodoItem)} by ID")
    .WithDescription($"Retrieves a single {nameof(TodoItem)} matching the specified ID. Returns 404 if not found.")
    .WithTags(Constants.TodoItemsTag)
    .Produces<TodoItem>()
    .Produces(StatusCodes.Status404NotFound);

todoItemEndpoints.MapPost("/", async (TodoItem todo, TodoDb db) =>
    {
        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        return Results.Created($"/{Constants.TodoItemsTag.ToLowerInvariant()}/{todo.Id}", todo);
    })
    .WithName("CreateTodo")
    .WithSummary($"Create a new {nameof(TodoItem)}")
    .WithDescription($"Adds a new {nameof(TodoItem)} to the database and returns the created item with its assigned ID.")
    .WithTags(Constants.TodoItemsTag)
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
    .WithTags(Constants.TodoItemsTag)
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
    .WithTags(Constants.TodoItemsTag)
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
    .WithTags(Constants.TodoItemsTag)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound);
#pragma warning restore CA2007
#pragma warning restore CA1308

await app.RunAsync().ConfigureAwait(false);