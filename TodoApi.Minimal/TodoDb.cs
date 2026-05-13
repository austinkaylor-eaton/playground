namespace TodoApi.Minimal;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents the Entity Framework Core database context for the to-do application, providing access to the to-do items stored in the database.
/// </summary>
/// <param name="options">The options used to configure the database context.</param>
public class TodoDb(DbContextOptions<TodoDb> options) : DbContext(options)
{
    /// <summary>
    /// Gets the DbSet of to-do items, allowing for querying and saving instances of the <see cref="TodoItem"/> entity.
    /// </summary>
    public DbSet<TodoItem> Todos => Set<TodoItem>();
}