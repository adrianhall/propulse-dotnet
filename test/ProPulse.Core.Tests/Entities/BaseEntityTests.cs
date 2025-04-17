using System;
using FluentAssertions;
using ProPulse.Core.Entities;
using Xunit;

namespace ProPulse.Core.Tests;

/// <summary>
/// Unit tests for <see cref="BaseEntity"/> equality and hash code behavior.
/// </summary>
public sealed class BaseEntityEqualityTests
{
    /// <summary>
    /// Tests that two entities of the same type with the same Id and Version are equal.
    /// </summary>
    [Fact]
    public void Entities_WithSameTypeIdAndVersion_AreEqual()
    {
        Guid id = Guid.NewGuid();
        byte[] version = [1, 2, 3];
        Article a = new() { Id = id, Version = version };
        Article b = new() { Id = id, Version = version };
        a.Should().Be(b);
        (a == b).Should().BeTrue();
        (a != b).Should().BeFalse();
        a.Equals(b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    /// <summary>
    /// Tests that two entities of different types with the same Id and Version are not equal.
    /// </summary>
    [Fact]
    public void Entities_WithDifferentTypes_AreNotEqual()
    {
        Guid id = Guid.NewGuid();
        byte[] version = [1, 2, 3];
        Article a = new() { Id = id, Version = version };
        TestEntity b = new() { Id = id, Version = version };
        a.Should().NotBe(b);
        (a == b).Should().BeFalse();
        (a != b).Should().BeTrue();
        a.Equals(b).Should().BeFalse();
    }

    /// <summary>
    /// Tests that two entities with different Ids or Versions are not equal.
    /// </summary>
    [Fact]
    public void Entities_WithDifferentIdOrVersion_AreNotEqual()
    {
        Article a = new() { Id = Guid.NewGuid(), Version = [1, 2, 3] };
        Article b = new() { Id = Guid.NewGuid(), Version = [1, 2, 3] };
        Article c = new() { Id = a.Id, Version = [4, 5, 6] };
        a.Should().NotBe(b);
        a.Should().NotBe(c);
    }

    /// <summary>
    /// Tests that an entity is equal to itself and not equal to null.
    /// </summary>
    [Fact]
    public void Entity_EqualsItself_AndNotNull()
    {
        Article a = new();
        a.Should().Be(a);
        a.Equals(null).Should().BeFalse();
        (a == null).Should().BeFalse();
        (null == a).Should().BeFalse();
    }

    /// <summary>
    /// Dummy entity for type comparison.
    /// </summary>
    private sealed class TestEntity : BaseEntity { }
}
