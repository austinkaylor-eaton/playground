using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Database;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemsController(TodoContext context) : ControllerBase
{
    // GET: api/TodoItems
    /// <summary>
    /// Gets all <see cref="TodoItem"/>s from the database.
    /// </summary>
    /// <returns>A list of <see cref="TodoItem"/>s.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
    {
        return await context.TodoItems.ToListAsync();
    }

    // GET: api/TodoItems/5
    /// <summary>
    /// Gets a specific <see cref="TodoItem"/> by ID from the database.
    /// </summary>
    /// <param name="id">The ID of the <see cref="TodoItem"/> to retrieve.</param>
    /// <returns>The <see cref="TodoItem"/> with the specified ID.</returns>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
    {
        var todoItem = await context.TodoItems.FindAsync(id);

        return todoItem == null ? NotFound() : todoItem;
    }

    // PUT: api/TodoItems/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    /// <summary>
    /// Updates a specific <see cref="TodoItem"/> by ID in the database.
    /// </summary>
    /// <param name="id">The ID of the <see cref="TodoItem"/> to update.</param>
    /// <param name="todoItem">The updated <see cref="TodoItem"/> object.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
    {
        if (id != todoItem.Id)
        {
            return BadRequest();
        }

        context.Entry(todoItem).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TodoItemExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    // POST: api/TodoItems
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    /// <summary>
    /// Creates a new <see cref="TodoItem"/> in the database.
    /// </summary>
    /// <param name="todoItem">The <see cref="TodoItem"/> object to create.</param>
    /// <returns>The created <see cref="TodoItem"/>.</returns>
    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
    {
        context.TodoItems.Add(todoItem);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
    }

    // DELETE: api/TodoItems/5
    /// <summary>
    /// Deletes a specific <see cref="TodoItem"/> by ID from the database.
    /// </summary>
    /// <param name="id">The ID of the <see cref="TodoItem"/> to delete.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteTodoItem(long id)
    {
        var todoItem = await context.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        context.TodoItems.Remove(todoItem);
        await context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Checks if the <see cref="TodoItem"/> exists in the database.
    /// </summary>
    /// <param name="id">The ID of the <see cref="TodoItem"/> to check.</param>
    /// <returns>True if the <see cref="TodoItem"/> exists; otherwise, false.</returns>
    private bool TodoItemExists(long id)
    {
        return context.TodoItems.Any(e => e.Id == id);
    }
}