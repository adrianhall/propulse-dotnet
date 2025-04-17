using ProPulse.Core.Utils;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProPulse.Core.Entities;

/// <summary>
/// The base class for all entities in the system.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// A unique identifier for the entity.
    /// </summary>
    public string Id { get; set; } = IdGenerator.NewId();

    /// <summary>
    /// The date and time when the entity was created.  This is stored
    /// in UTC format with microsecond precision.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The date and time when the entity was deleted.  This is stored
    /// in UTC format with microsecond precision, if set.  If not set,
    /// then the entity is not deleted.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// The date and time when the entity was last updated. This is stored
    /// in UTC format with microsecond precision.  This is automatically
    /// maintained by the database and is used to provide the Last-Modified
    /// header when the entity is returned in a response.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// The row version for the entity. This is automatically maintained
    /// by the database to ensure concurrency control and is used to provide
    /// the ETag header when the entity is returned in a response.
    /// </summary>
    public byte[] Version { get; set; } = [];
}