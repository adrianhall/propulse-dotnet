using Microsoft.Extensions.Logging;
using ProPulse.DataModel.Initializer.Interfaces;

namespace ProPulse.DataModel.Initializer.Services;

/// <summary>
/// Concrete implementation of the <see cref="IDatabaseInitializerService"/> interface.
/// </summary>
/// <param name="migrator">The migrator service (via dependency injection)</param>
/// <param name="logger">The logger (via dependency injection)</param>
public class DatabaseInitializerService(IDatabaseMigratorService migrator, ILogger<DatabaseInitializerService> logger) : IDatabaseInitializerService
{
    /// <summary>
    /// Applies the database migrations.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting database migration...");
        try
        {
            logger.LogInformation("Connection string: {connectionString}", migrator.ConnectionString);
            await migrator.ApplyMigrationsAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Applied scripts: {scripts}", string.Join(", ", migrator.AppliedScripts));
            logger.LogInformation("Database migrations applied.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Migration failed");
            throw;
        }
    }
}