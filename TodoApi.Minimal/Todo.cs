using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Minimal;

/// <summary>
/// Represents a to-do item in the database with an identifier, name, and completion status.
/// </summary>
public class Todo
{
    /// <summary>
    /// Gets or sets the unique identifier for the to-do item.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the to-do item.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the to-do item is complete.
    /// </summary>
    public bool IsComplete { get; set; }
}