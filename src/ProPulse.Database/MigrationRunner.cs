using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace ProPulse.Database;

/// <summary>
/// Handles database migrations using DbUp.
/// </summary>
public class MigrationRunner([NotNull] string connectionString, [NotNull] ILogger<MigrationRunner> logger)
{
    /// <summary>
    /// The schema where the migration journal table will be created.
    /// </summary>
    private const string MigrationSchema = "public";

    /// <summary>
    /// The name of the migration journal table.
    /// </summary>
    private const string MigrationTable = "__schema_migrations";

    /// <summary>
    /// Creates a configured DbUp upgrader instance.
    /// </summary>
    /// <returns>A DbUp upgrader instance configured for the ProPulse database.</returns>
    private UpgradeEngine CreateUpgrader()
    {
        return DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithTransaction()
            .LogToConsole()
            .LogTo(logger)
            .WithProPulseScripts()
            .JournalToPostgresqlTable(MigrationSchema, MigrationTable)
            .Build();
    }

    /// <summary>
    /// Runs all pending database migrations.
    /// </summary>
    /// <returns>True if migrations were successful, false otherwise.</returns>
    public bool RunMigrations()
    {
        logger.LogInformation("Starting database migrations...");

        // Log all scripts that will be executed
        var scriptsToExecute = GetPendingMigrations().ToList();
        logger.LogInformation("Found {ScriptCount} scripts to execute", scriptsToExecute.Count);

        foreach (var script in scriptsToExecute)
        {
            logger.LogInformation("Script found: {ScriptName}", script);
        }

        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        DatabaseUpgradeResult result = CreateUpgrader().PerformUpgrade();
        if (!result.Successful)
        {
            logger.LogError(result.Error, "Database migration failed");
            return false;
        }

        logger.LogInformation("Database migrations completed successfully");
        return true;
    }

    /// <summary>
    /// Gets the list of scripts that will be executed on the next migration run.
    /// </summary>
    /// <returns>A collection of script names that will be executed.</returns>
    public IEnumerable<string> GetPendingMigrations()
        => CreateUpgrader().GetScriptsToExecute().Select(s => s.Name);

    /// <summary>
    /// Logs all embedded script resources found in the assembly for debugging purposes.
    /// </summary>
    internal void LogEmbeddedScriptResources()
    {
        logger.LogInformation("Checking for embedded SQL script resources in assembly...");

        var assembly = typeof(MigrationRunner).Assembly;
        var resourceNames = assembly.GetManifestResourceNames();

        logger.LogInformation("Found {ResourceCount} total resources in assembly", resourceNames.Length);

        foreach (var resourceName in resourceNames)
        {
            logger.LogInformation("Resource found: {ResourceName}", resourceName);
        }

        // Specifically look for SQL files
        var sqlResources = resourceNames.Where(r => r.EndsWith(".sql", StringComparison.OrdinalIgnoreCase)).ToList();
        logger.LogInformation("Found {SqlCount} SQL resources in assembly", sqlResources.Count);

        foreach (var sqlResource in sqlResources)
        {
            logger.LogInformation("SQL resource found: {SqlResource}", sqlResource);
        }
    }

    /// <summary>
    /// Resets the database by executing the reset script.
    /// </summary>
    /// <returns>True if the reset was successful, false otherwise.</returns>
    public bool ResetDatabase()
    {
        logger.LogInformation("Starting database reset...");
        logger.LogWarning("This will delete all schemas and data in the database!");

        try
        {
            // The resource name follows the namespace hierarchy and file path
            const string resetScriptResourceName = "ProPulse.Database.Scripts.Utility.reset-database.sql";
            string? resetScriptContent = GetEmbeddedResourceContent(resetScriptResourceName);

            if (string.IsNullOrEmpty(resetScriptContent))
            {
                logger.LogError("Failed to load the reset script resource: {ResourceName}", resetScriptResourceName);
                return false;
            }

            // Execute the script directly using Npgsql
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = resetScriptContent;
            command.ExecuteNonQuery();

            logger.LogInformation("Database reset completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database reset failed");
            return false;
        }
    }

    /// <summary>
    /// Retrieves the content of an embedded resource.
    /// </summary>
    /// <param name="resourceName">The full name of the embedded resource.</param>
    /// <returns>The content of the embedded resource as a string, or null if the resource couldn't be found.</returns>
    private string? GetEmbeddedResourceContent(string resourceName)
    {
        Assembly assembly = typeof(MigrationRunner).Assembly;

        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            logger.LogError("Embedded resource not found: {ResourceName}", resourceName);
            return null;
        }

        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
