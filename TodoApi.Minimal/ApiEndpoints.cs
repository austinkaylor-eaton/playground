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
    /// Registers the endpoints for the <see cref="TodoItem"/>s resource, such as the URL paths and HTTP methods for each endpoint, with a <see cref="RouteGroupBuilder"/> instead of a <see cref="WebApplication"/>.
    /// </summary>
    /// <param name="group">The <see cref="RouteGroupBuilder"/> to which the endpoints will be added.</param>
    /// <returns>The <see cref="RouteGroupBuilder"/> with the registered endpoints.</returns>
    public static void MapTodoItemEndpoints(this RouteGroupBuilder group)
    {
        group.WithTags(Constants.TodoItems.EndpointGroupTag);
        group.WithGroupName(Constants.TodoItems.EndpointGroupTag);
        group.WithSummary($"Endpoints for managing {nameof(TodoItem)}s");
        group.MapGet("/", GetAllTodos);
        group.MapGet("/{id:int}", GetTodoById);
        group.MapGet("/complete", GetCompleteTodos);
        group.MapPost("/", CreateTodo);
        group.MapPut("/{id:int}", UpdateTodo);
        group.MapPatch("/{id:int}", PatchTodo);
        group.MapDelete("/{id:int}", DeleteTodo);
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

        return TypedResults.Created($"/{Constants.TodoItems.EndpointGroupTag}/{createdDto.Id}", createdDto);
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