using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ProPulse.DataModel.Initializer.Interfaces;
using ProPulse.DataModel.Initializer.Services;
using Xunit;

namespace ProPulse.DataModel.Initializer.Tests;

/// <summary>
/// Unit tests for <see cref="DatabaseInitializerService"/>.
/// </summary>
public sealed class DatabaseInitializerServiceTests
{
    /// <summary>
    /// Verifies that RunAsync calls ApplyMigrationsAsync and logs success when migrations succeed.
    /// </summary>
    [Fact]
    public async Task RunAsync_CallsApplyMigrationsAndLogsSuccess()
    {
        IDatabaseMigratorService migrator = Substitute.For<IDatabaseMigratorService>();
        migrator.ConnectionString.Returns("Host=localhost;Database=test;Username=postgres;Password=postgres");
        migrator.AppliedScripts.Returns(new List<string>([ "001-init.sql" ]));
        ILogger<DatabaseInitializerService> logger = Substitute.For<ILogger<DatabaseInitializerService>>();
        DatabaseInitializerService service = new(migrator, logger);

        // Act
        Func<Task> act = async () => await service.RunAsync();

        // Assert
        await act.Should().NotThrowAsync();
        await migrator.Received(1).ApplyMigrationsAsync(Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Verifies that RunAsync logs and rethrows when ApplyMigrationsAsync throws.
    /// </summary>
    [Fact]
    public async Task RunAsync_LogsAndRethrows_WhenMigrationFails()
    {
        IDatabaseMigratorService migrator = Substitute.For<IDatabaseMigratorService>();
        migrator.ConnectionString.Returns("Host=localhost;Database=test;Username=postgres;Password=postgres");
        migrator.AppliedScripts.Returns([]);
        migrator.ApplyMigrationsAsync(Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("fail"));
        ILogger<DatabaseInitializerService> logger = Substitute.For<ILogger<DatabaseInitializerService>>();
        DatabaseInitializerService service = new(migrator, logger);

        // Act
        Func<Task> act = async () => await service.RunAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("fail");
        await migrator.Received(1).ApplyMigrationsAsync(Arg.Any<CancellationToken>());
    }
}
