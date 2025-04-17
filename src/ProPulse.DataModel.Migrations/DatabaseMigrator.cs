using System.Data.Common;
using System.Reflection;
using DbUp;
using DbUp.Builder;
using DbUp.Engine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;

namespace ProPulse.DataModel.Migrations;

/// <summary>
/// Applies embedded SQL migration scripts to a PostgreSQL database and tracks applied migrations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DatabaseMigrator"/> class.
/// </remarks>
public class DatabaseMigrator
{
    /// <summary>
    /// Gets or sets the logger for migration operations.
    /// </summary>
    public ILogger Logger { get; set; } = NullLogger.Instance;

    /// <summary>
    /// Gets the list of applied migration script names.
    /// </summary>
    public List<string> AppliedScripts { get; } = [];

    /// <summary>
    /// Gets the list of discovered migration script names.
    /// </summary>
    public List<string> DiscoveredScripts { get; } = [];

    /// <summary>
    /// The assembly that contains the embedded migration scripts.
    /// </summary>
    public Assembly ContainingAssembly { get; }

    /// <summary>
    /// The prefix for the embedded resource names.
    /// </summary>
    public string ResourcePrefix { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseMigrator"/> class.
    /// </summary>
    /// <param name="assembly">The assembly containing the embedded migration scripts.</param>
    /// <param name="resourcePrefix">The prefix for the embedded resource names. If null, defaults to the assembly name plus ".Scripts."</param>
    public DatabaseMigrator(Assembly? assembly = null, string? resourcePrefix = null)
    {
        ContainingAssembly = assembly ?? Assembly.GetExecutingAssembly();
        ResourcePrefix = resourcePrefix ?? ContainingAssembly.GetName().Name + ".Scripts.";
    }

    /// <summary>
    /// Applies all pending migrations to the database specified by the connection string.
    /// </summary>
    /// <param name="connectionString">The PostgreSQL connection string.</param>
    public void ApplyMigrations(string connectionString)
    {
        Logger.LogInformation("Ensuring database exists.");
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        Logger.LogInformation("Applying migrations to database.");
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(ContainingAssembly, ResourcePrefix)
            .LogTo(Logger)
            .LogScriptOutput()
            .WithTransactionPerScript()
            .JournalToPostgresqlTable("public", "DbUpSchemaVersion")
            .Build();

        // Determine the scripts that are discovered.
        var scripts = upgrader.GetDiscoveredScripts();
        foreach (var script in scripts)
        {
            DiscoveredScripts.Add(script.Name);
        }

        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        foreach (var script in result.Scripts)
        {
            AppliedScripts.Add(script.Name);
        }
        LogAndThrowErrors(result);
    }

    internal void LogAndThrowErrors(DatabaseUpgradeResult result)
    {
        if (result.Successful)
        {
            return;
        }

        Logger.LogError(result.Error, "Database migration failed at script {Script}: {Error}",
            result.ErrorScript?.Name, result.Error?.Message);

        throw new DatabaseMigrationException("Database migration failed", result.Error);
    }
}

/// <summary>
/// A helper class for DbUp to filter embedded resources.
/// </summary>
internal static class DbUpLoggerExtensions
{
    /// <summary>
    /// Adds scripts from the provided assembly using a filter to the embedded resource scripts.
    /// </summary>
    /// <param name="builder">The builder to modify.</param>
    /// <param name="assembly">The assembly containing the embedded scripts.</param>
    /// <param name="resourcePrefix">The prefix for the embedded resource names.</param>
    /// <returns>The builder, for chaining.</returns>
    internal static UpgradeEngineBuilder WithScriptsEmbeddedInAssembly(this UpgradeEngineBuilder builder, Assembly assembly, string resourcePrefix)
    {
        return builder.WithScriptsEmbeddedInAssembly(assembly, s => s.StartsWith(resourcePrefix, StringComparison.OrdinalIgnoreCase) && s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase));
    }
}

