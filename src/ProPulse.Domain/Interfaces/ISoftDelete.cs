namespace ProPulse.Domain.Interfaces;

/// <summary>
/// Interface for entities that support soft delete functionality.
/// Implementing this interface allows for standardized soft delete operations
/// across the application.
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is soft deleted.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was soft deleted.
    /// Will be null when the entity is not deleted.
    /// </summary>
    DateTimeOffset? DeletedAt { get; set; }
}
