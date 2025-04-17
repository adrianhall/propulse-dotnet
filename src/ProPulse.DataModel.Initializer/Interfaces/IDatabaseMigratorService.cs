namespace ProPulse.DataModel.Initializer.Interfaces;

/// <summary>
/// An interface describing a service for applying database migrations.
/// </summary>
public interface IDatabaseMigratorService
{
    /// <summary>
    /// The list of applied SQL scripts.
    /// </summary>
    List<string> AppliedScripts { get; }

    /// <summary>
    /// The connection string to be used to connect to the database.
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// Applies the migrations to the database specified by the connection string.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ApplyMigrationsAsync(CancellationToken cancellationToken = default);
}