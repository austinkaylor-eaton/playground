using JetBrains.Annotations;

namespace TodoApi.Minimal;

/// <summary>
/// A <see href="https://en.wikipedia.org/wiki/Data_transfer_object">data transfer object</see>
/// representing a <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Methods/PATCH">PATCH</see> request for a <see cref="TodoItem"/>.
/// </summary>
/// <remarks>
/// This is used to decouple the internal database representation of a TodoItem from the external representation sent to clients,
/// allowing for more flexibility in changing the database schema without affecting the API contract with clients. <br/>
/// </remarks>
[PublicAPI]
public class TodoItemPatchDTO
{
    /// <inheritdoc cref="TodoItem.Name"/>
    public string? Name { get; set; }

    /// <inheritdoc cref="TodoItem.IsComplete"/>
    public bool? IsComplete { get; set; }

    /// <inheritdoc cref="ConvertToDatabaseEntity"/>
    public static implicit operator TodoItem(TodoItemPatchDTO? dto) => ConvertToDatabaseEntity(dto);

    /// <inheritdoc cref="ConvertToDatabaseEntity"/>
    public static TodoItem ToTodoItem(TodoItemPatchDTO? dto) => ConvertToDatabaseEntity(dto);

    /// <summary>
    /// Converts a <see cref="TodoItemPatchDTO"/> to a <see cref="TodoItem"/>. <br/>
    /// </summary>
    /// <param name="dto">The <see cref="TodoItemPatchDTO"/> to convert.</param>
    /// <returns>A <see cref="TodoItem"/> instance.</returns>
    private static TodoItem ConvertToDatabaseEntity(TodoItemPatchDTO? dto)
    {
        return dto != null ? new TodoItem { Name = dto.Name, IsComplete = dto.IsComplete ?? false } : new TodoItem();
    }

}