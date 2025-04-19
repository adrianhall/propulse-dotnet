using DotNet.Testcontainers.Containers;
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

    /// <summary>
    /// Indicates whether migrations have been applied to the database.
    /// </summary>
    public bool MigrationsApplied { get; private set; } = false;

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
        if (withMigrations)
        {
            var migrator = new DatabaseMigrator { Logger = _container.Logger };
            migrator.ApplyMigrations(ConnectionString);
            await ReloadTypesAsync();
            MigrationsApplied = true;
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

    /// <summary>
    /// Gets the list of installed extensions in the PostgreSQL database.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe.</param>
    /// <returns>A dictionary with the installed extensions.</returns>
    public async Task<Dictionary<string, string>> GetInstalledExtensionsAsync(CancellationToken cancellationToken = default)
    {
        string query = "SELECT * FROM pg_available_extensions WHERE installed_version IS NOT NULL;";
        List<Dictionary<string, object>> result = await ExecuteQueryAsync(query, cancellationToken);
        return result.ToDictionary(row => row["name"].ToString()!, row => row["installed_version"].ToString()!);
    }

    /// <summary>
    /// This reloads the types in the NpgsqlConnection so that future queries will use
    /// the latest one.  It gets around a bug in the migration + EF Core world where the
    /// extension list is cached after the migration.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ReloadTypesAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync(cancellationToken);
        await conn.ReloadTypesAsync(cancellationToken);
        await conn.CloseAsync();
    }
}
