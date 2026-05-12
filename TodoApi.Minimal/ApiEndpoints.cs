using Microsoft.EntityFrameworkCore;

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
        group.MapGet("/{id:long}", GetTodoById);
        group.MapGet("/complete", GetCompleteTodos);
        group.MapPost("/", CreateTodo);
        group.MapPut("/{id:long}", UpdateTodo);
        group.MapPatch("/{id:long}", PatchTodo);
        group.MapDelete("/{id:long}", DeleteTodo);
    }

    /// <summary>
    /// Gets all <see cref="TodoItem"/>s from the database
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <response code="200">Returns an array of all TodoItems</response>>
    private static async Task<IResult> GetAllTodos(TodoDb db)
    {
        var todos = await db.Todos
            .Select(todoItem => TodoItemDTO.ToTodoItemDTO(todoItem))
            .ToArrayAsync();
        return TypedResults.Ok(todos);
    }

    /// <summary>
    /// Gets all completed <see cref="TodoItem"/>s from the database
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <response code="200">Returns an array of all completed TodoItems</response>
    private static async Task<IResult> GetCompleteTodos(TodoDb db)
    {
        var todos = await db.Todos
            .Where(t => t.IsComplete)
            .Select(todoItem => TodoItemDTO.ToTodoItemDTO(todoItem))
            .ToArrayAsync();
        return TypedResults.Ok(todos);
    }

    /// <summary>
    /// Gets a single <see cref="TodoItem"/> by its ID from the database
    /// </summary>
    /// <param name="id">The ID of the <see cref="TodoItem"/> to retrieve.</param>
    /// <param name="db">The database context.</param>
    /// <response code="200">Returns the TodoItem with the specified ID</response>
    /// <response code="404">If a TodoItem with the specified ID is not found</response>
    private static async Task<IResult> GetTodoById(long id, TodoDb db)
    {
        var todoItem = await db.Todos.FindAsync(id);

        return todoItem is null ?
            TypedResults.NotFound() :
            TypedResults.Ok(TodoItemDTO.ToTodoItemDTO(todoItem));
    }

    /// <summary>
    /// Creates a new <see cref="TodoItem"/> in the database based on the provided <see cref="TodoItemDTO"/>
    /// </summary>
    /// <param name="todoItemDTO">The <see cref="TodoItemDTO"/> containing the data for the new <see cref="TodoItem"/></param>
    /// <param name="db">The database context</param>
    /// <response code="201">Returns the newly created TodoItem</response>
    /// <remarks>
    /// The location header of the response will contain the URL of the newly created item.
    /// </remarks>
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
    /// Updates an existing <see cref="TodoItem"/> in the database based on the provided ID and <see cref="TodoItemDTO"/>
    /// </summary>
    /// <param name="id">The ID of the <see cref="TodoItem"/> to update.</param>
    /// <param name="todoItemDTO">The <see cref="TodoItemDTO"/> containing the updated data.</param>
    /// <param name="db">The database context.</param>
    /// <response code="400">If the ID in the URL does not match the ID in the request body</response>
    /// <response code="404">If a TodoItem with the specified ID is not found</response>
    /// <response code="204">If the update is successful</response>
    private static async Task<IResult> UpdateTodo(long id, TodoItemDTO todoItemDTO, TodoDb db)
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
    /// Partially updates an existing <see cref="TodoItem"/> in the database based on the provided ID and <see cref="TodoItemPatchDTO"/>
    /// </summary>
    /// <param name="id">The ID of the <see cref="TodoItem"/> to update.</param>
    /// <param name="todoItemDTO">The <see cref="TodoItemPatchDTO"/> containing the updated data.</param>
    /// <param name="db">The database context.</param>
    /// <response code="404">If a TodoItem with the specified ID is not found</response>
    /// <response code="204">If the update is successful</response>
    private static async Task<IResult> PatchTodo(long id, TodoItemPatchDTO todoItemDTO, TodoDb db)
    {
        var todo = await db.Todos.FindAsync(id);

        if (todo is null) return Results.NotFound();

        if (todoItemDTO.Name is not null) todo.Name = todoItemDTO.Name;
        if (todoItemDTO.IsComplete is not null) todo.IsComplete = todoItemDTO.IsComplete.Value;

        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    /// <summary>
    /// Deletes an existing <see cref="TodoItem"/> from the database based on the provided ID
    /// </summary>
    /// <param name="id">The ID of the <see cref="TodoItem"/> to delete.</param>
    /// <param name="db">The database context.</param>
    /// <response code="404">If a TodoItem with the specified ID is not found</response>
    /// <response code="204">If the deletion is successful</response>
    private static async Task<IResult> DeleteTodo(long id, TodoDb db)
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