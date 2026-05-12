using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Constants = TodoApi.Minimal.Constants;

namespace TodoApi.Minimal;

/// <summary>
/// Contains methods for registering and interacting with the API endpoints,
/// such as the URL paths and HTTP methods for each endpoint,
/// as well as any additional metadata that may be useful for documentation or testing purposes
/// </summary>
public static class ApiEndpoints
{
    /// <summary>
    /// Registers the endpoints for the <see cref="TodoItem"/>s resource, such as the URL paths and HTTP methods for each endpoint,
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance to register the endpoints with.</param>
    public static void RegisterTodoItemsEndpoints(this WebApplication app)
    {
        var todoItemEndpoints = app.MapGroup($"/{Constants.TodoItems.Tag.ToLowerInvariant()}")
            .WithTags(Constants.TodoItems.Tag);

        todoItemEndpoints.MapGet("/", async (TodoDb db) =>
                await db.Todos.ToListAsync())
            .WithName("GetAllTodos")
            .WithSummary($"Get all {nameof(TodoItem)}s")
            .WithDescription($"Retrieves the complete list of {nameof(TodoItem)}s from the database.")
            .Produces<List<TodoItem>>();

        todoItemEndpoints.MapGet("/complete", async (TodoDb db) =>
                await db.Todos.Where(t => t.IsComplete).ToListAsync())
            .WithName("GetCompleteTodos")
            .WithSummary($"Get completed {nameof(TodoItem)}s")
            .WithDescription($"Retrieves only {nameof(TodoItem)}s that have been marked as complete.")
            .Produces<List<TodoItem>>();

        todoItemEndpoints.MapGet("/{id:int}", async Task<Results<Ok<TodoItem>, NotFound>>(int id, TodoDb db) =>
                await db.Todos.FindAsync(id)
                    is { } todo
                    ? TypedResults.Ok(todo)
                    : TypedResults.NotFound())
            .WithName("GetTodoById")
            .WithSummary($"Get a {nameof(TodoItem)} by ID")
            .WithDescription($"Retrieves a single {nameof(TodoItem)} matching the specified ID. Returns 404 if not found.")
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
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Registers the endpoints for the <see cref="TodoItem"/>s resource, such as the URL paths and HTTP methods for each endpoint, with a <see cref="RouteGroupBuilder"/> instead of a <see cref="WebApplication"/>.
    /// </summary>
    /// <param name="group">The <see cref="RouteGroupBuilder"/> to which the endpoints will be added.</param>
    /// <returns>The <see cref="RouteGroupBuilder"/> with the registered endpoints.</returns>
    public static RouteGroupBuilder MapTodoItemEndpoints(this RouteGroupBuilder group)
    {
        group.WithTags(Constants.TodoItems.Tag);
        group.WithGroupName(Constants.TodoItems.Tag);
        group.MapGet("/", GetAllTodos);
        group.MapGet("/{id:int}", GetTodoById);
        group.MapGet("/complete", GetCompleteTodos);
        group.MapPost("/", CreateTodo);
        group.MapPut("/{id:int}", UpdateTodo);
        group.MapPatch("/{id:int}", PatchTodo);
        group.MapDelete("/{id:int}", DeleteTodo);

        return group;
    }

    /// <summary>
    /// Gets all <see cref="TodoItem"/>s from the database and returns them as an array of <see cref="TodoItemDTO"/>s.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <returns>An array of <see cref="TodoItemDTO"/>s.</returns>
    private static async Task<IResult> GetAllTodos(TodoDb db)
    {
        var todos = await db.Todos
            .Select(todoItem => TodoItemDTO.ToTodoItemDTO(todoItem))
            .ToArrayAsync();
        return TypedResults.Ok(todos);
    }

    /// <summary>
    /// Gets all completed <see cref="TodoItem"/>s from the database and returns them as an array of <see cref="TodoItemDTO"/>s.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <returns>An array of <see cref="TodoItemDTO"/>s.</returns>
    private static async Task<IResult> GetCompleteTodos(TodoDb db)
    {
        var todos = await db.Todos
            .Where(t => t.IsComplete)
            .Select(todoItem => TodoItemDTO.ToTodoItemDTO(todoItem))
            .ToArrayAsync();
        return TypedResults.Ok(todos);
    }

    /// <summary>
    /// Gets a single <see cref="TodoItem"/> by its ID from the database and returns it as a <see cref="TodoItemDTO"/>.
    /// If the item is not found, returns a 404 Not Found response.
    /// </summary>
    /// <param name="id">The ID of the <see cref="TodoItem"/> to retrieve.</param>
    /// <param name="db">The database context.</param>
    /// <returns>A <see cref="TodoItemDTO"/> if found; otherwise, a 404 Not Found response.</returns>
    private static async Task<IResult> GetTodoById(int id, TodoDb db)
    {
        var todoItem = await db.Todos.FindAsync(id);

        return todoItem == null ?
            TypedResults.NotFound() :
            TypedResults.Ok(todoItem);
    }

    /// <summary>
    /// Creates a new <see cref="TodoItem"/> in the database based on the provided <see cref="TodoItemDTO"/>
    /// and returns the created item as a <see cref="TodoItemDTO"/> with a 201 Created response.
    /// The location header of the response will contain the URL of the newly created item.
    /// </summary>
    /// <param name="todoItemDTO">The <see cref="TodoItemDTO"/> containing the data for the new <see cref="TodoItem"/>.</param>
    /// <param name="db">The database context.</param>
    /// <returns>The created <see cref="TodoItemDTO"/> with a 201 Created response.</returns>
    private static async Task<IResult> CreateTodo(TodoItemDTO todoItemDTO, TodoDb db)
    {
        // Convert the DTO to a database entity explicitly so we can capture the database-generated ID after saving.
        // The implicit conversion creates a new TodoItem instance; adding the DTO directly would lose the reference
        // to the tracked entity, causing todoItem.Id to remain 0 in the response.
        TodoItem dbEntity = TodoItemDTO.ToTodoItem(todoItemDTO);
        db.Todos.Add(dbEntity);
        await db.SaveChangesAsync();

        TodoItemDTO createdDto = TodoItemDTO.ToTodoItemDTO(dbEntity);

        return TypedResults.Created($"/{Constants.TodoItems.Tag}/{createdDto.Id}", createdDto);
    }

    /// <summary>
    /// Updates an existing <see cref="TodoItem"/> in the database based on the provided ID and <see cref="TodoItemDTO"/>.
    /// If the ID in the URL does not match the ID in the DTO, returns a 400 Bad Request response.
    /// If the item with the specified ID does not exist, returns a 404 Not Found response. If the update is successful, returns a 204 No Content response.
    /// </summary>
    /// <param name="id">The ID of the <see cref="TodoItem"/> to update.</param>
    /// <param name="todoItemDTO">The <see cref="TodoItemDTO"/> containing the updated data.</param>
    /// <param name="db">The database context.</param>
    /// <returns>A 204 No Content response if the update is successful; otherwise, a 400 Bad Request or 404 Not Found response.</returns>
    private static async Task<IResult> UpdateTodo(int id, TodoItemDTO todoItemDTO, TodoDb db)
    {
        if (id != todoItemDTO.Id)
            return TypedResults.BadRequest();

        var todo = await db.Todos.FindAsync(id);

        if (todo is null) return TypedResults.NotFound();

        todo.Name = todoItemDTO.Name;
        todo.IsComplete = todoItemDTO.IsComplete;

        db.Entry(todo).State = EntityState.Modified;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    /// <summary>
    /// Partially updates an existing <see cref="TodoItem"/> in the database based on the provided ID and <see cref="TodoItemPatchDTO"/>.
    /// If the item with the specified ID does not exist, returns a 404 Not Found response. If the update is successful, returns a 204 No Content response.
    /// </summary>
    /// <param name="id">The ID of the <see cref="TodoItem"/> to update.</param>
    /// <param name="todoItemDTO">The <see cref="TodoItemPatchDTO"/> containing the updated data.</param>
    /// <param name="db">The database context.</param>
    /// <returns>A 204 No Content response if the update is successful; otherwise, a 404 Not Found response.</returns>
    private static async Task<IResult> PatchTodo(int id, TodoItemPatchDTO todoItemDTO, TodoDb db)
    {
        var todo = await db.Todos.FindAsync(id);

        if (todo is null) return Results.NotFound();

        if (todoItemDTO.Name is not null) todo.Name = todoItemDTO.Name;
        if (todoItemDTO.IsComplete is not null) todo.IsComplete = todoItemDTO.IsComplete.Value;

        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    /// <summary>
    /// Deletes an existing <see cref="TodoItem"/> from the database based on the provided ID.
    /// If the item with the specified ID does not exist, returns a 404 Not Found response. If the deletion is successful, returns a 204 No Content response.
    /// </summary>
    /// <param name="id">The ID of the <see cref="TodoItem"/> to delete.</param>
    /// <param name="db">The database context.</param>
    /// <returns>A 204 No Content response if the deletion is successful; otherwise, a 404 Not Found response.</returns>
    private static async Task<IResult> DeleteTodo(int id, TodoDb db)
    {
        if (await db.Todos.FindAsync(id) is not { } todo)
        {
            return TypedResults.NotFound();
        }

        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}