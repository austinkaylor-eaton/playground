namespace Core;

/// <summary>
/// Allows an entity to be activated or deactivated.
/// </summary>
public interface IActivateEntity
{
    /// <summary>
    /// Indicates whether the entity is currently active.
    /// </summary>
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