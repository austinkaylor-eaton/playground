namespace TodoApi.Minimal;

using Microsoft.EntityFrameworkCore;

public class TodoDb(DbContextOptions<TodoDb> options) : DbContext(options)
{
    /// <summary>
    /// Gets the DbSet of to-do items, allowing for querying and saving instances of the <see cref="TodoItem"/> entity.
    /// </summary>
    public DbSet<TodoItem> Todos => Set<TodoItem>();
}