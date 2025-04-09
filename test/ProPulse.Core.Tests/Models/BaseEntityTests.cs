using System.Text;
using FluentAssertions;
using ProPulse.Core.Models;

namespace ProPulse.Core.Tests.Models;

/// <summary>
/// Tests for the <see cref="BaseEntity"/> class.
/// </summary>
public class BaseEntityTests
{
    /// <summary>
    /// Concrete implementation of BaseEntity for testing purposes.
    /// </summary>
    private class TestEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets a name for the test entity.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }    /// <summary>
    /// Tests that Equals returns true for the same entity reference.
    /// </summary>
    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test" };

        // Act & Assert
        entity.Equals(entity).Should().BeTrue();
        #pragma warning disable CS1718 // Comparison made to same variable
        (entity == entity).Should().BeTrue();
        #pragma warning restore CS1718 // Comparison made to same variable
    }

    /// <summary>
    /// Tests that Equals returns false when comparing with null.
    /// </summary>
    [Fact]
    public void Equals_NullEntity_ReturnsFalse()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test" };
        TestEntity? nullEntity = null;

        // Act & Assert
        entity.Equals(nullEntity).Should().BeFalse();
        entity!.Equals((object?)null).Should().BeFalse();
        (entity == nullEntity).Should().BeFalse();
        (nullEntity == entity).Should().BeFalse();
    }

    /// <summary>
    /// Tests that two null entities are considered equal.
    /// </summary>
    [Fact]
    public void Equals_BothNull_ReturnsTrue()
    {
        // Arrange
        TestEntity? nullEntity1 = null;
        TestEntity? nullEntity2 = null;

        // Act & Assert
        (nullEntity1 == nullEntity2).Should().BeTrue();
    }

    /// <summary>
    /// Tests that Equals returns true for entities with identical properties.
    /// </summary>
    [Fact]
    public void Equals_EntitiesWithIdenticalProperties_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var updatedAt = DateTimeOffset.UtcNow;
        var version = Encoding.UTF8.GetBytes("1.0.0");

        var entity1 = new TestEntity
        {
            Id = id,
            UpdatedAt = updatedAt,
            Version = version
        };

        var entity2 = new TestEntity
        {
            Id = id,
            UpdatedAt = updatedAt,
            Version = version
        };

        // Act & Assert
        entity1.Equals(entity2).Should().BeTrue();
        entity1.Equals((object)entity2).Should().BeTrue();
        (entity1 == entity2).Should().BeTrue();
        (entity1 != entity2).Should().BeFalse();
    }

    /// <summary>
    /// Tests that Equals returns false for entities with different Ids.
    /// </summary>
    [Fact]
    public void Equals_DifferentIds_ReturnsFalse()
    {
        // Arrange
        var updatedAt = DateTimeOffset.UtcNow;
        var version = Encoding.UTF8.GetBytes("1.0.0");

        var entity1 = new TestEntity
        {
            Id = Guid.NewGuid().ToString(),
            UpdatedAt = updatedAt,
            Version = version
        };

        var entity2 = new TestEntity
        {
            Id = Guid.NewGuid().ToString(),
            UpdatedAt = updatedAt,
            Version = version
        };

        // Act & Assert
        entity1.Equals(entity2).Should().BeFalse();
        (entity1 == entity2).Should().BeFalse();
        (entity1 != entity2).Should().BeTrue();
    }

    /// <summary>
    /// Tests that Equals returns false for entities with different UpdatedAt values.
    /// </summary>
    [Fact]
    public void Equals_DifferentUpdatedAtValues_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var version = Encoding.UTF8.GetBytes("1.0.0");

        var entity1 = new TestEntity
        {
            Id = id,
            UpdatedAt = DateTimeOffset.UtcNow,
            Version = version
        };

        var entity2 = new TestEntity
        {
            Id = id,
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(1),
            Version = version
        };

        // Act & Assert
        entity1.Equals(entity2).Should().BeFalse();
        (entity1 == entity2).Should().BeFalse();
        (entity1 != entity2).Should().BeTrue();
    }

    /// <summary>
    /// Tests that Equals returns false for entities with different Version values.
    /// </summary>
    [Fact]
    public void Equals_DifferentVersionValues_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var updatedAt = DateTimeOffset.UtcNow;

        var entity1 = new TestEntity
        {
            Id = id,
            UpdatedAt = updatedAt,
            Version = Encoding.UTF8.GetBytes("1.0.0")
        };

        var entity2 = new TestEntity
        {
            Id = id,
            UpdatedAt = updatedAt,
            Version = Encoding.UTF8.GetBytes("2.0.0")
        };

        // Act & Assert
        entity1.Equals(entity2).Should().BeFalse();
        (entity1 == entity2).Should().BeFalse();
        (entity1 != entity2).Should().BeTrue();
    }

    /// <summary>
    /// Tests that Equals handles null Version values correctly.
    /// </summary>
    [Fact]
    public void Equals_NullVersions_HandledCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var updatedAt = DateTimeOffset.UtcNow;

        var entity1 = new TestEntity
        {
            Id = id,
            UpdatedAt = updatedAt,
            Version = null
        };

        var entity2 = new TestEntity
        {
            Id = id,
            UpdatedAt = updatedAt,
            Version = null
        };

        var entity3 = new TestEntity
        {
            Id = id,
            UpdatedAt = updatedAt,
            Version = Encoding.UTF8.GetBytes("1.0.0")
        };

        // Act & Assert
        entity1.Equals(entity2).Should().BeTrue();
        entity1.Equals(entity3).Should().BeFalse();
        entity3.Equals(entity1).Should().BeFalse();
    }

    /// <summary>
    /// Tests that GetHashCode returns the same value for equal entities.
    /// </summary>
    [Fact]
    public void GetHashCode_EqualEntities_ReturnsSameHashCode()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var updatedAt = DateTimeOffset.UtcNow;
        var version = Encoding.UTF8.GetBytes("1.0.0");

        var entity1 = new TestEntity
        {
            Id = id,
            UpdatedAt = updatedAt,
            Version = version
        };

        var entity2 = new TestEntity
        {
            Id = id,
            UpdatedAt = updatedAt,
            Version = version
        };

        // Act & Assert
        entity1.GetHashCode().Should().Be(entity2.GetHashCode());
    }

    /// <summary>
    /// Tests that GetHashCode returns different values for entities with different properties.
    /// </summary>
    [Fact]
    public void GetHashCode_DifferentEntities_ReturnsDifferentHashCodes()
    {
        // Arrange
        var entity1 = new TestEntity
        {
            Id = Guid.NewGuid().ToString(),
            UpdatedAt = DateTimeOffset.UtcNow,
            Version = Encoding.UTF8.GetBytes("1.0.0")
        };

        var entity2 = new TestEntity
        {
            Id = Guid.NewGuid().ToString(),
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(1),
            Version = Encoding.UTF8.GetBytes("2.0.0")
        };

        // Act & Assert
        entity1.GetHashCode().Should().NotBe(entity2.GetHashCode());
    }

    /// <summary>
    /// Tests that GetHashCode handles null Version correctly.
    /// </summary>
    [Fact]
    public void GetHashCode_NullVersion_HandledCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var updatedAt = DateTimeOffset.UtcNow;

        var entity = new TestEntity
        {
            Id = id,
            UpdatedAt = updatedAt,
            Version = null
        };

        // Act & Assert - Should not throw an exception
        var hashCode = entity.GetHashCode();
        hashCode.Should().NotBe(0);
    }

    /// <summary>
    /// Tests that the Version hash code is sensitive to changes in any byte of the array.
    /// </summary>
    [Fact]
    public void VersionHashCode_SensitiveToAllBytes()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var updatedAt = DateTimeOffset.UtcNow;

        // Create a base version array
        var baseVersion = new byte[] { 1, 2, 3, 4, 5 };

        var baseEntity = new TestEntity
        {
            Id = id,
            UpdatedAt = updatedAt,
            Version = baseVersion
        };

        var baseHashCode = baseEntity.GetHashCode();

        // Test that changing any byte in the version affects the hash code
        for (int i = 0; i < baseVersion.Length; i++)
        {
            // Create a modified version with one byte changed
            var modifiedVersion = new byte[baseVersion.Length];
            Array.Copy(baseVersion, modifiedVersion, baseVersion.Length);
            modifiedVersion[i] = (byte)(baseVersion[i] + 1);

            var modifiedEntity = new TestEntity
            {
                Id = id,
                UpdatedAt = updatedAt,
                Version = modifiedVersion
            };

            var modifiedHashCode = modifiedEntity.GetHashCode();

            // Assert that changing any byte in the version affects the hash code
            modifiedHashCode.Should().NotBe(baseHashCode, 
                $"Hash code should change when byte at index {i} is modified");
        }
    }
}
