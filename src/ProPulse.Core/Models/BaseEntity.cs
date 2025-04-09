using System.Diagnostics.CodeAnalysis;
using ProPulse.Core.Models.Identity;

namespace ProPulse.Core.Models;

/// <summary>
/// Base abstract class for all entities in the ProPulse system.
/// Provides common properties and functionality for audit tracking and versioning.
/// </summary>
public abstract class BaseEntity : IEquatable<BaseEntity>
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

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is BaseEntity entity && Equals(entity);
    }

    /// <summary>
    /// Determines whether the specified entity is equal to the current entity.
    /// Entities are considered equal if their Id, UpdatedAt, and Version are identical.
    /// </summary>
    /// <param name="other">The entity to compare with the current entity.</param>
    /// <returns>true if the specified entity is equal to the current entity; otherwise, false.</returns>
    public bool Equals(BaseEntity? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id == other.Id &&
               UpdatedAt.Equals(other.UpdatedAt) &&
               VersionEquals(Version, other.Version);
    }

    /// <summary>
    /// Gets the hash code for this entity based on its Id, UpdatedAt, and Version.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Id,
            UpdatedAt,
            Version is not null ? GetVersionHashCode(Version) : 0);
    }

    /// <summary>
    /// Determines whether two entities are equal.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns>true if the entities are equal; otherwise, false.</returns>
    public static bool operator ==(BaseEntity? left, BaseEntity? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two entities are not equal.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns>true if the entities are not equal; otherwise, false.</returns>
    public static bool operator !=(BaseEntity? left, BaseEntity? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Compares two version arrays for equality.
    /// </summary>
    /// <param name="version1">The first version array.</param>
    /// <param name="version2">The second version array.</param>
    /// <returns>true if the version arrays are equal; otherwise, false.</returns>
    private static bool VersionEquals(byte[]? version1, byte[]? version2)
    {
        if (version1 is null && version2 is null)
        {
            return true;
        }

        if (version1 is null || version2 is null)
        {
            return false;
        }

        if (version1.Length != version2.Length)
        {
            return false;
        }

        return version1.SequenceEqual(version2);
    }    /// <summary>
    /// Computes a hash code for a version array by considering all bytes in the array.
    /// </summary>
    /// <param name="version">The version array.</param>
    /// <returns>A hash code for the version array.</returns>
    private static int GetVersionHashCode(byte[] version)
    {
        if (version.Length == 0)
        {
            return 0;
        }

        // Use all bytes in the version array for the hash code
        int hash = 17;
        foreach (byte b in version)
        {
            hash = hash * 31 + b;
        }
        
        return hash;
    }
}
