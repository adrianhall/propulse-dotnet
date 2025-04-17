using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ProPulse.DataModel.Initializer.Services;
using ProPulseTests.Common;
using Xunit;

namespace ProPulse.DataModel.Initializer.Tests;

/// <summary>
/// Unit tests for <see cref="DatabaseMigratorService"/>.
/// </summary>
public class DatabaseMigratorServiceTests(PostgreSqlContainerFixture fixture) : IClassFixture<PostgreSqlContainerFixture>
{
    /// <summary>
    /// Verifies that ApplyMigrationsAsync applies migrations when a valid connection string is provided.
    /// </summary>
    [Fact]
    public async Task ApplyMigrationsAsync_AppliesMigrations_WhenConnectionStringProvided()
    {
        await fixture.EnsureInitializedAsync(withMigrations: false);

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = fixture.ConnectionString
            })
            .Build();
        ILogger<DatabaseMigratorService> logger = Substitute.For<ILogger<DatabaseMigratorService>>();
        DatabaseMigratorService service = new(configuration, logger);

        // Act
        Func<Task> act = async () => await service.ApplyMigrationsAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    /// <summary>
    /// Verifies that ApplyMigrationsAsync throws when no connection string is provided.
    /// </summary>
    [Fact]
    public async Task ApplyMigrationsAsync_Throws_WhenConnectionStringMissing()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([])
            .Build();
        ILogger<DatabaseMigratorService> logger = Substitute.For<ILogger<DatabaseMigratorService>>();

        // Act
        Func<Task> act = async () =>
        {
            DatabaseMigratorService service = new(configuration, logger);
            await service.ApplyMigrationsAsync();
        };

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Connection string not found.*");
    }
}
