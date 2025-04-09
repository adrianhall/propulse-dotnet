namespace ProPulse.Core;

/// <summary>
/// Interface for entities that support user auditing.
/// Implements tracking of which users created and updated an entity.
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// Gets or sets the ID of the user who created this entity.
    /// </summary>
    string? CreatedById { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the user who last updated this entity.
    /// </summary>
    string? UpdatedById { get; set; }
}
