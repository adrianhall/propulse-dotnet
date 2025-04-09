using System.Diagnostics.CodeAnalysis;
using ProPulse.Core.Models.Identity;

namespace ProPulse.Core.Models;

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
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "This property is used as a concurrency token in EF Core")]
    public byte[]? Version { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who created this entity.
    /// </summary>
    public string? CreatedById { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who last updated this entity.
    /// </summary>
    public string? UpdatedById { get; set; }
    
    /// <summary>
    /// Gets or sets the user who created this entity.
    /// </summary>
    public virtual ApplicationUser? CreatedBy { get; set; }
    
    /// <summary>
    /// Gets or sets the user who last updated this entity.
    /// </summary>
    public virtual ApplicationUser? UpdatedBy { get; set; }
}
