namespace Core;

/// <summary>
/// Allows an entity to be activated or deactivated.
/// </summary>
/// <remarks>
/// Implement this interface on entities that support a soft enable/disable lifecycle
/// without being deleted from the system.
/// </remarks>
/// <example>
/// Implement activation on a domain entity:
/// <code>
/// public class Product : Entity&lt;Guid&gt;, IActivateEntity
/// {
///     public bool IsActive { get; private set; }
///
///     public void Activate() =&gt; IsActive = true;
///
///     public void Deactivate() =&gt; IsActive = false;
/// }
/// </code>
/// </example>
/// <seealso cref="ISoftDeleteEntity"/>
public interface IActivateEntity
{
    /// <summary>
    /// Gets a value indicating whether the entity is currently active.
    /// </summary>
    /// <value><c>true</c> if the entity is active; otherwise, <c>false</c>.</value>
    bool IsActive { get; }

    /// <summary>
    /// Activates the entity, setting its state to active.
    /// </summary>
    void Activate();

    /// <summary>
    /// Deactivates the entity, setting its state to inactive.
    /// </summary>
    void Deactivate();
}

