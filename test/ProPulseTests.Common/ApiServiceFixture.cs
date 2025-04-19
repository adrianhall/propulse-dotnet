using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProPulse.DataModel;
using ProPulse.Core.Entities;
using Xunit;
using System.Diagnostics.CodeAnalysis;

namespace ProPulseTests.Common;

/// <summary>
/// Provides a fixture for API integration tests that sets up a PostgreSQL test container and a WebApplicationFactory for the specified API program.
/// </summary>
/// <typeparam name="TProgram">The entry point type of the API application.</typeparam>
[ExcludeFromCodeCoverage]
public sealed class ApiServiceFixture<TProgram> : IAsyncLifetime where TProgram : class
{
    /// <summary>
    /// Gets the PostgreSQL test container fixture.
    /// </summary>
    public PostgreSqlContainerFixture PostgreSqlFixture { get; } = new();

    /// <summary>
    /// Gets the WebApplicationFactory for the API.
    /// </summary>
    public WebApplicationFactory<TProgram> ServiceFactory { get; private set; } = default!;

    /// <summary>
    /// Gets an HttpClient for making requests to the API.
    /// </summary>
    public HttpClient ServiceClient { get; private set; } = default!;

    /// <summary>
    /// The list of articles that were seeded into the database.
    /// </summary>
    public List<Article> Articles { get; private set; } = [];

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        await PostgreSqlFixture.EnsureInitializedAsync(withMigrations: true);
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", PostgreSqlFixture.ConnectionString);

        // Seed 75 articles
        DbContextOptionsBuilder<AppDbContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql(PostgreSqlFixture.ConnectionString);
        using (AppDbContext context = new(optionsBuilder.Options))
        {
            if (!await context.Articles.AnyAsync())
            {
                List<Article> articles = [];
                for (int i = 1; i <= 75; i++)
                {
                    articles.Add(new Article
                    {
                        Title = $"Seeded Article {i}",
                        Content = $"This is the content for seeded article {i}."
                    });
                }
                await context.Articles.AddRangeAsync(articles);
                await context.SaveChangesAsync();
                Articles = await context.Articles.AsNoTracking().ToListAsync();
            }
        }

        ServiceFactory = new WebApplicationFactory<TProgram>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(
                    [
                        new KeyValuePair<string, string?>("ConnectionStrings:DefaultConnection", PostgreSqlFixture.ConnectionString)
                    ]);
                });
            });

        ServiceClient = ServiceFactory.CreateClient();
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        ServiceClient?.Dispose();
        ServiceFactory?.Dispose();
        await PostgreSqlFixture.DisposeAsync();
    }

    /// <summary>
    /// Gets a random article from the seeded list of articles.
    /// </summary>
    /// <returns>A random article.</returns>
    public Article GetRandomArticle()
    {
        Random random = new();
        int index = random.Next(Articles.Count);
        return Articles[index];
    }

    /// <summary>
    /// Soft deletes an article in the database.
    /// </summary>
    /// <param name="article">The article to soft-delete</param>
    /// <returns>The updated</returns>
    public async Task<Article> SoftDeleteArticleAsync(Article article)
    {
        await using AppDbContext context = new(new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(PostgreSqlFixture.ConnectionString)
            .Options);
        article.IsDeleted = true;
        context.Articles.Update(article);
        await context.SaveChangesAsync();
        return await context.Articles.IgnoreQueryFilters().AsNoTracking().SingleAsync(a => a.Id == article.Id);
    }
}
