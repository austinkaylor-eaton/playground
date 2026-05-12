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
app.UseOpenApi();
app.UseSwaggerUi();

#endregion

#pragma warning disable CA2007
app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todos.ToListAsync());

app.MapGet("/todoitems/complete", async (TodoDb db) =>
    await db.Todos.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/todoitems/{id:int}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id)
        is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

app.MapPost("/todoitems", async (TodoItem todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id:int}", async (int id, TodoItem inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todoitems/{id:int}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is not { } todo)
    {
        return Results.NotFound();
    }

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();

});
#pragma warning restore CA2007

await app.RunAsync().ConfigureAwait(false);