using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using ProPulse.DataModel.Migrations;
using System.Diagnostics.CodeAnalysis;
using Testcontainers.PostgreSql;
using Xunit;

namespace ProPulseTests.Common;

/// <summary>
/// Provides a shared PostgreSQL Testcontainer for integration tests, with optional migration application.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class PostgreSqlContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("testdb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private bool _migrationsApplied;

    /// <summary>
    /// Gets the PostgreSQL connection string.
    /// </summary>
    public string ConnectionString => _container.GetConnectionString();

    /// <summary>
    /// Ensures the container is started and migrations are applied if requested.
    /// </summary>
    /// <param name="withMigrations">If true, applies migrations after starting the container. Default is true.</param>
    public async Task EnsureInitializedAsync(bool withMigrations = true)
    {
        if (_container.State == TestcontainersStates.Running)
        {
            return;
        }

        await _container.StartAsync();
        if (withMigrations && !_migrationsApplied)
        {
            var migrator = new DatabaseMigrator { Logger = NullLogger.Instance };
            migrator.ApplyMigrations(ConnectionString);
            _migrationsApplied = true;
        }
    }

    /// <inheritdoc />
    public Task InitializeAsync()
        => Task.CompletedTask;
        //=> EnsureInitializedAsync();

    /// <inheritdoc />
    public async Task DisposeAsync() => await _container.DisposeAsync();

    /// <summary>
    /// Executes a query and returns the result as a list of dictionaries.
    /// </summary>
    public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string query, CancellationToken cancellationToken = default)
    {
        await using NpgsqlConnection connection = new(ConnectionString);
        await connection.OpenAsync(cancellationToken);

        List<Dictionary<string, object>> result = [];
        await using NpgsqlCommand command = new(query, connection);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            Dictionary<string, object> row = [];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.GetValue(i);
            }
            result.Add(row);
        }
        return result;
    }
}
