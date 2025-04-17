using System;
using FluentAssertions;
using ProPulse.Core.Entities;
using Xunit;

namespace ProPulse.Core.Tests.Entities;

/// <summary>
/// Unit tests for the <see cref="Article"/> entity model.
/// </summary>
public sealed class ArticleTests
{
    /// <summary>
    /// Tests that a new Article has its properties set to expected default values.
    /// </summary>
    [Fact]
    public void Article_DefaultProperties_AreSetAsExpected()
    {
        // Arrange
        string expectedTitle = "Test Title";
        string expectedContent = "Test Content";
        DateTimeOffset before = DateTimeOffset.UtcNow;
        Article article = new Article { Title = expectedTitle, Content = expectedContent };
        DateTimeOffset after = DateTimeOffset.UtcNow;

        // Assert
        article.Id.Should().NotBeEmpty();
        article.Title.Should().Be(expectedTitle);
        article.Content.Should().Be(expectedContent);
        article.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        article.PublishedAt.Should().BeNull();
        article.IsDeleted.Should().BeFalse();
        article.DeletedAt.Should().BeNull();
    }
}
