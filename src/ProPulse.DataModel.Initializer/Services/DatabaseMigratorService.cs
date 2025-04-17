using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProPulse.DataModel.Initializer.Interfaces;
using ProPulse.DataModel.Migrations;

namespace ProPulse.DataModel.Initializer.Services;

/// <summary>
/// A wrapped implementation of the database migrator that uses configuration passed in
/// via the application's configuration settings.
/// </summary>
public class DatabaseMigratorService : IDatabaseMigratorService
{
    private readonly DatabaseMigrator _migrator;

    /// <summary>
    /// Creates a new instance of the <see cref="DatabaseMigratorService"/> class using the
    /// connection string given in <c>DefaultConnection</c> in the configuration settings.
    /// </summary>
    /// <param name="configuration">The configuration settings.</param>
    /// <param name="logger">The </param>
    /// <exception cref="InvalidOperationException"></exception>
    public DatabaseMigratorService(IConfiguration configuration, ILogger<DatabaseMigratorService> logger)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection") ??
                                   throw new InvalidOperationException("Connection string not found.");
        _migrator = new DatabaseMigrator() { Logger = logger };
    }

    /// <summary>
    /// The list of applied SQL scripts.
    /// </summary>
    public List<string> AppliedScripts { get => _migrator.AppliedScripts; }

    /// <summary>
    /// The connection string to be used to connect to the database.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// Applies the migrations to the database specified by the connection string.
    /// </summary>
    /// <param name="connectionString">The c</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ApplyMigrationsAsync(CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            _migrator.ApplyMigrations(ConnectionString);
        }, cancellationToken).ConfigureAwait(false);
    }
}