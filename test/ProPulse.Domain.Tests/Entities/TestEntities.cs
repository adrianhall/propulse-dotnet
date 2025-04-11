using System;
using ProPulse.Domain.Entities;

namespace ProPulse.Domain.Tests.Entities;

/// <summary>
/// Test implementation of BaseEntity for unit testing.
/// </summary>
public class TestEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets a test property to differentiate entities.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Creates a new instance of the TestEntity class with the specified values.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="updatedAt">The last update timestamp.</param>
    /// <param name="name">The test name property.</param>
    /// <returns>A fully initialized test entity.</returns>
    public static TestEntity Create(Guid id, DateTimeOffset updatedAt, string? name = null)
    {
        return new TestEntity
        {
            Id = id,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
            UpdatedAt = updatedAt,
            Name = name
        };
    }
}

/// <summary>
/// A second test entity type to test equality between different entity types.
/// </summary>
public class AnotherTestEntity : BaseEntity
{
    /// <summary>
    /// Creates a new instance of the AnotherTestEntity class with the specified values.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="updatedAt">The last update timestamp.</param>
    /// <returns>A fully initialized test entity.</returns>
    public static AnotherTestEntity Create(Guid id, DateTimeOffset updatedAt)
    {
        return new AnotherTestEntity
        {
            Id = id,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
            UpdatedAt = updatedAt
        };
    }
}
