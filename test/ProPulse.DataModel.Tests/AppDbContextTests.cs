using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProPulse.Core.Entities;
using ProPulseTests.Common;

namespace ProPulse.DataModel.Tests;

/// <summary>
/// Integration tests for <see cref="AppDbContext"/> using a PostgreSQL test container.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AppDbContextTests"/> class.
/// </remarks>
/// <param name="fixture">The PostgreSQL test container fixture.</param>
[Collection("TestContainerCollection")]
public sealed class AppDbContextTests(PostgreSqlContainerFixture fixture) : IClassFixture<PostgreSqlContainerFixture>
{
    /// <summary>
    /// Tests that an article can be created and the audit fields are set by the database.
    /// </summary>
    [Fact]
    public async Task Can_create_article_and_audit_fields_are_set()
    {
        await fixture.EnsureInitializedAsync(withMigrations: true);
        await using AppDbContext context = CreateDbContext();
        var extensions = await fixture.GetInstalledExtensionsAsync();
        extensions.Should().ContainKey("citext");

        // Create a new article and save it to the database
        Article article = new()
        {
            Title = "Test Article",
            Content = "Hello, world!"
        };
        context.Articles.Add(article);
        DateTimeOffset expectedDate = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync();

        Article? dbArticle = await context.Articles.FirstOrDefaultAsync(a => a.Id == article.Id);
        dbArticle.Should().NotBeNull();
        dbArticle!.CreatedAt.Should().BeCloseTo(expectedDate, TimeSpan.FromSeconds(5));
        dbArticle.UpdatedAt.Should().BeCloseTo(expectedDate, TimeSpan.FromSeconds(5));
        dbArticle.DeletedAt.Should().BeNull();
        dbArticle.IsDeleted.Should().BeFalse();
        dbArticle.Version.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that updating an article updates the UpdatedAt field and the Version.
    /// </summary>
    [Fact]
    public async Task Can_update_article_and_updatedat_is_changed()
    {
        await fixture.EnsureInitializedAsync(withMigrations: true);
        await using AppDbContext context = CreateDbContext();

        Article article = new()
        {
            Title = "Update Test",
            Content = "Initial"
        };
        context.Articles.Add(article);
        await context.SaveChangesAsync();

        DateTimeOffset originalCreatedAt = article.CreatedAt;
        DateTimeOffset originalUpdatedAt = article.UpdatedAt;
        DateTimeOffset expectedPublishedAt = DateTimeOffset.UtcNow.AddDays(1);
        byte[] originalVersion = [.. article.Version];

        article.Content = "Updated content";
        article.PublishedAt = expectedPublishedAt;
        DateTimeOffset expectedDate = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync();

        Article? dbArticle = await context.Articles.FirstOrDefaultAsync(a => a.Id == article.Id);
        dbArticle.Should().NotBeNull();
        dbArticle!.CreatedAt.Should().Be(originalCreatedAt);
        dbArticle.UpdatedAt.Should().BeCloseTo(expectedDate, TimeSpan.FromSeconds(5));
        dbArticle.DeletedAt.Should().BeNull();
        dbArticle.IsDeleted.Should().BeFalse();
        dbArticle.PublishedAt.Should().Be(expectedPublishedAt);
        dbArticle.Version.Should().NotBeNullOrEmpty().And.NotBeEquivalentTo(originalVersion);
    }

    /// <summary>
    /// Tests that soft-deleting an article sets IsDeleted and DeletedAt, and that the global filter hides it. Also checks Version changes.
    /// </summary>
    [Fact]
    public async Task Can_soft_delete_article_and_global_filter_applies()
    {
        await fixture.EnsureInitializedAsync(withMigrations: true);
        await using AppDbContext context = CreateDbContext();

        Article article = new()
        {
            Title = "Delete Test",
            Content = "To be deleted"
        };
        context.Articles.Add(article);
        await context.SaveChangesAsync();

        DateTimeOffset originalCreatedAt = article.CreatedAt;
        DateTimeOffset originalUpdatedAt = article.UpdatedAt;
        byte[] originalVersion = [.. article.Version];
        article.IsDeleted = true;
        DateTimeOffset expectedDate = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync();

        Article? dbArticle = await context.Set<Article>().IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Id == article.Id);
        dbArticle.Should().NotBeNull();
        dbArticle!.CreatedAt.Should().Be(originalCreatedAt);
        dbArticle.UpdatedAt.Should().BeCloseTo(expectedDate, TimeSpan.FromSeconds(5));
        dbArticle.IsDeleted.Should().BeTrue();
        dbArticle.DeletedAt.Should().NotBeNull();
        dbArticle.Version.Should().NotBeNullOrEmpty().And.NotBeEquivalentTo(originalVersion);


        // Should not be returned by default query
        (await context.Articles.FirstOrDefaultAsync(a => a.Id == article.Id)).Should().BeNull();
    }

    /// <summary>
    /// Tests that un-deleting an article clears DeletedAt and restores it to queries. Also checks Version changes.
    /// </summary>
    [Fact]
    public async Task Can_undelete_article_and_global_filter_restores()
    {
        await fixture.EnsureInitializedAsync(withMigrations: true);
        await using AppDbContext context = CreateDbContext();
        Article article = new()
        {
            Title = "Undelete Test",
            Content = "To be restored",
            IsDeleted = true,
            DeletedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };
        context.Articles.Add(article);
        await context.SaveChangesAsync();

        DateTimeOffset originalCreatedAt = article.CreatedAt;
        DateTimeOffset originalUpdatedAt = article.UpdatedAt;
        byte[] originalVersion = [.. article.Version];
        article.IsDeleted = false;
        DateTimeOffset expectedDate = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync();

        Article? dbArticle = await context.Articles.FirstOrDefaultAsync(a => a.Id == article.Id);
        dbArticle.Should().NotBeNull();
        dbArticle!.CreatedAt.Should().Be(originalCreatedAt);
        dbArticle.UpdatedAt.Should().BeCloseTo(expectedDate, TimeSpan.FromSeconds(5));
        dbArticle.IsDeleted.Should().BeFalse();
        dbArticle.DeletedAt.Should().BeNull();
        dbArticle.Version.Should().NotBeNullOrEmpty().And.NotBeEquivalentTo(originalVersion);

    }

    private AppDbContext CreateDbContext()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(fixture.ConnectionString)
            .Options;
        return new AppDbContext(options);
    }
}
