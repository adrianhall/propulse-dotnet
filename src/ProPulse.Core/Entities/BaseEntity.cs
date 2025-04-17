namespace ProPulse.Core.Entities;

using System;
using System.Collections.Generic;

/// <summary>
/// The base class for all entities in the system.
/// </summary>
public abstract class BaseEntity : IEquatable<BaseEntity>
{
    /// <summary>
    /// A unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; } = Guid.CreateVersion7();

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
    /// Indicates whether the entity is soft-deleted. If true, the entity is considered deleted
    /// and will be filtered from queries by default.
    /// </summary>
    public bool IsDeleted { get; set; }

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

    #region IEquatable<BaseEntity> Members
    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as BaseEntity);

    /// <inheritdoc />
    public bool Equals(BaseEntity? other)
    {
        if (ReferenceEquals(this, other))
            return true;
        if (other is null)
            return false;
        return GetType() == other.GetType()
            && Id == other.Id
            && EqualityComparer<byte[]>.Default.Equals(Version, other.Version);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(GetType());
        hash.Add(Id);
        if (Version is not null)
        {
            foreach (byte b in Version)
                hash.Add(b);
        }
        return hash.ToHashCode();
    }

    /// <summary>
    /// Determines whether two entities are equal.
    /// </summary>
    /// <param name="left">The first entity.</param>
    /// <param name="right">The second entity.</param>
    /// <returns>True if the entities are equal; otherwise, false.</returns>
    public static bool operator ==(BaseEntity? left, BaseEntity? right)
        => EqualityComparer<BaseEntity>.Default.Equals(left, right);

    /// <summary>
    /// Determines whether two entities are not equal.
    /// </summary>
    /// <param name="left">The first entity.</param>
    /// <param name="right">The second entity.</param>
    /// <returns>True if the entities are not equal; otherwise, false.</returns>
    public static bool operator !=(BaseEntity? left, BaseEntity? right)
        => !(left == right);
    #endregion
}