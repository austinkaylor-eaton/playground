using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace TodoApi.Minimal;

/// <summary>
/// A <see href="https://en.wikipedia.org/wiki/Data_transfer_object">data transfer object</see> representing a <see cref="TodoItem"/> to be sent to clients of the API.
/// </summary>
/// <remarks>
/// This is used to decouple the internal database representation of a TodoItem from the external representation sent to clients, allowing for more flexibility in changing the database schema without affecting the API contract with clients. <br/>
/// </remarks>
[PublicAPI]
public class TodoItemDTO
{
    /// <inheritdoc cref="TodoItem.Id"/>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long Id { get; init; }

    /// <inheritdoc cref="TodoItem.Name"/>
    [StringLength(100)]
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string? Name { get; init; }

    /// <inheritdoc cref="TodoItem.IsComplete"/>
    [JsonPropertyName("isComplete")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool IsComplete { get; init; }

    /// <inheritdoc cref="ConvertToDatabaseEntity(TodoItemDTO)"/>
    public static implicit operator TodoItem(TodoItemDTO? dto) => ConvertToDatabaseEntity(dto);

    /// <inheritdoc cref="ConvertToDataTransferObject(TodoItem)"/>
    public static implicit operator TodoItemDTO(TodoItem? dbEntity) => ConvertToDataTransferObject(dbEntity);

    /// <inheritdoc cref="ConvertToDatabaseEntity(TodoItemDTO)"/>
    public static TodoItem ToTodoItem(TodoItemDTO? dto) => ConvertToDatabaseEntity(dto);

    /// <inheritdoc cref="ConvertToDataTransferObject(TodoItem)"/>
    public static TodoItemDTO ToTodoItemDTO(TodoItem? dbEntity) => ConvertToDataTransferObject(dbEntity);

    /// <summary>
    /// Converts a <see cref="TodoItemDTO"/> to a <see cref="TodoItem"/>. <br/>
    /// </summary>
    /// <param name="dto">The <see cref="TodoItemDTO"/> to convert.</param>
    /// <returns>The converted <see cref="TodoItem"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dto"/> is null.</exception>
    private static TodoItem ConvertToDatabaseEntity(TodoItemDTO? dto)
    {
        return dto != null ?
            new TodoItem { Id = dto.Id, Name = dto.Name, IsComplete = dto.IsComplete } :
            throw new ArgumentNullException(nameof(dto));
    }

    /// <summary>
    /// Converts a <see cref="TodoItem"/> to a <see cref="TodoItemDTO"/>. <br/>
    /// </summary>
    /// <param name="dbEntity">The <see cref="TodoItem"/> to convert.</param>
    /// <returns>The converted <see cref="TodoItemDTO"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="dbEntity"/> is null.</exception>
    private static TodoItemDTO ConvertToDataTransferObject(TodoItem? dbEntity)
    {
        return dbEntity != null ?
            new TodoItemDTO { Id = dbEntity.Id, Name = dbEntity.Name, IsComplete = dbEntity.IsComplete } :
            throw new ArgumentNullException(nameof(dbEntity));
    }
}