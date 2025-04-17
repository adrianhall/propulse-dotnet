namespace ProPulse.DataModel.Initializer.Interfaces;

public interface IDatabaseInitializerService
{
    /// <summary>
    /// Applies the database migrations.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RunAsync(CancellationToken cancellationToken = default);
}