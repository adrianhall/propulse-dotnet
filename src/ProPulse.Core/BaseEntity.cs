namespace ProPulse.Core;

/// <summary>
/// Base abstract class for all entities in the ProPulse system.
/// Provides common properties and functionality for audit tracking and versioning.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the version number used for concurrency control.
    /// </summary>
    public byte[]? Version { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who created this entity.
    /// </summary>
    public string? CreatedById { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who last updated this entity.
    /// </summary>
    public string? UpdatedById { get; set; }
}
