using FluentAssertions;

namespace ProPulse.Domain.Tests.Entities;

/// <summary>
/// Unit tests for the BaseEntity class focusing on equality and hash code functionality.
/// </summary>
public class BaseEntityTests
{
    private readonly Guid _id1 = Guid.NewGuid();
    private readonly Guid _id2 = Guid.NewGuid();
    private readonly DateTimeOffset _timestamp1 = new DateTimeOffset(2025, 4, 10, 10, 0, 0, TimeSpan.Zero);
    private readonly DateTimeOffset _timestamp2 = new DateTimeOffset(2025, 4, 10, 11, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Equals_WithSameIdAndTimestamp_ReturnsTrue()
    {
        // Arrange
        var entity1 = TestEntity.Create(_id1, _timestamp1, "Entity 1");
        var entity2 = TestEntity.Create(_id1, _timestamp1, "Entity 2");  // Different name, same ID and timestamp

        // Act & Assert
        entity1.Should().Be(entity2);
    }

    [Fact]
    public void Equals_WithDifferentId_ReturnsFalse()
    {
        // Arrange
        var entity1 = TestEntity.Create(_id1, _timestamp1);
        var entity2 = TestEntity.Create(_id2, _timestamp1);  // Different ID, same timestamp

        // Act & Assert
        entity1.Should().NotBe(entity2);
    }

    [Fact]
    public void Equals_WithDifferentTimestamp_ReturnsFalse()
    {
        // Arrange
        var entity1 = TestEntity.Create(_id1, _timestamp1);
        var entity2 = TestEntity.Create(_id1, _timestamp2);  // Same ID, different timestamp

        // Act & Assert
        entity1.Should().NotBe(entity2);
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var entity = TestEntity.Create(_id1, _timestamp1);

        // Act & Assert
        entity.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentEntityType_ReturnsFalse()
    {
        // Arrange
        var entity1 = TestEntity.Create(_id1, _timestamp1);
        var entity2 = AnotherTestEntity.Create(_id1, _timestamp1);  // Same ID and timestamp, different type

        // Act & Assert
        entity1.Equals(entity2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNonEntityObject_ReturnsFalse()
    {
        // Arrange
        var entity = TestEntity.Create(_id1, _timestamp1);
        var nonEntity = "Not an entity";

        // Act & Assert
        entity.Equals(nonEntity).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithSameReference_ReturnsTrue()
    {
        // Arrange
        var entity = TestEntity.Create(_id1, _timestamp1);

        // Act & Assert - Testing object.Equals(object) implementation
        entity.Equals((object)entity).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_WithSameIdAndTimestamp_ReturnsSameHashCode()
    {
        // Arrange
        var entity1 = TestEntity.Create(_id1, _timestamp1, "Entity 1");
        var entity2 = TestEntity.Create(_id1, _timestamp1, "Entity 2");  // Different name, same ID and timestamp

        // Act & Assert
        entity1.GetHashCode().Should().Be(entity2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentIdOrTimestamp_ReturnsDifferentHashCode()
    {
        // Arrange
        var entity1 = TestEntity.Create(_id1, _timestamp1);
        var entity2 = TestEntity.Create(_id2, _timestamp1);  // Different ID
        var entity3 = TestEntity.Create(_id1, _timestamp2);  // Different timestamp

        // Act & Assert
        entity1.GetHashCode().Should().NotBe(entity2.GetHashCode());
        entity1.GetHashCode().Should().NotBe(entity3.GetHashCode());
    }

    [Fact]
    public void EqualsOperator_WithSameEntities_ReturnsTrue()
    {
        // Arrange
        var entity1 = TestEntity.Create(_id1, _timestamp1);
        var entity2 = TestEntity.Create(_id1, _timestamp1);

        // Act & Assert
        (entity1 == entity2).Should().BeTrue();
    }

    [Fact]
    public void EqualsOperator_WithDifferentEntities_ReturnsFalse()
    {
        // Arrange
        var entity1 = TestEntity.Create(_id1, _timestamp1);
        var entity2 = TestEntity.Create(_id2, _timestamp1);

        // Act & Assert
        (entity1 == entity2).Should().BeFalse();
    }

    [Fact]
    public void EqualsOperator_WithNull_ReturnsFalse()
    {
        // Arrange
        var entity = TestEntity.Create(_id1, _timestamp1);

        // Act & Assert
        (entity == null).Should().BeFalse();
        (null == entity).Should().BeFalse();
    }

    [Fact]
    public void EqualsOperator_WithBothNull_ReturnsTrue()
    {
        // Arrange
        TestEntity? entity1 = null;
        TestEntity? entity2 = null;

        // Act & Assert
        (entity1 == entity2).Should().BeTrue();
    }

    [Fact]
    public void NotEqualsOperator_ReturnsOppositeOfEqualsOperator()
    {
        // Arrange
        var entity1 = TestEntity.Create(_id1, _timestamp1);
        var entity2 = TestEntity.Create(_id1, _timestamp1);
        var entity3 = TestEntity.Create(_id2, _timestamp1);
        TestEntity? nullEntity = null;

        // Act & Assert
        (entity1 != entity2).Should().BeFalse();  // Same entities
        (entity1 != entity3).Should().BeTrue();   // Different entities
        (entity1 != nullEntity).Should().BeTrue(); // Entity vs null
        (nullEntity != entity1).Should().BeTrue(); // Null vs entity

        // Suppress CS1718 warning - Comparison made to same variable
#pragma warning disable CS1718 // Comparison made to same variable
        (nullEntity != nullEntity).Should().BeFalse(); // Null vs null
#pragma warning restore CS1718 // Comparison made to same variable
    }
}
