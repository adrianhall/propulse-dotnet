using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProPulse.Core.Options;

namespace ProPulse.DataModel;

/// <summary>
/// Provides extension methods for registering the <see cref="AppDbContext"/> in an ASP.NET Core application's service collection.
/// </summary>
public static class DbContextServiceCollectionExtensions
{
    /// <summary>
    /// The name of the connection string to use.
    /// </summary>
    private const string ConnectionStringName = "DefaultConnection";

    /// <summary>
    /// The name of the section in the configuration that contains the database options.
    /// </summary>
    private const string DatabaseOptionsSectionName = "Database:Options";

    /// <summary>
    /// Adds the <see cref="AppDbContext"/> to the service collection using PostgreSQL, pulling the 
    /// connection string from the provided configuration.  It also injects an <see cref="IOptions{DatabaseOptions}"/> 
    /// instance with options pulled from the configuration.
    /// </summary>
    /// <param name="services">The services model.</param>
    /// <param name="configuration">The configuration to use.</param>
    /// <returns></returns>
    public static IServiceCollection AddDataModel(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString(ConnectionStringName) ??
            throw new ArgumentException($"Connection string '{ConnectionStringName}' not found.");
        var databaseOptions = new DatabaseOptions();
        configuration.GetSection(DatabaseOptionsSectionName).Bind(databaseOptions);
        return services.AddDataModel(connectionString, databaseOptions);
    }

    /// <summary>
    /// Adds the <see cref="AppDbContext"/> to the service collection using PostgreSQL and the provided 
    /// connection string, using default options.
    /// </summary>
    /// <param name="services">The services connection.</param>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static IServiceCollection AddDataModel(this IServiceCollection services, string connectionString)
        => services.AddDataModel(connectionString, new DatabaseOptions());

    /// <summary>
    /// Adds the <see cref="AppDbContext"/> to the service collection using PostgreSQL and the provided connection string.
    /// </summary>
    /// <param name="services">The service collection to add the context to.</param>
    /// <param name="connectionString">The PostgreSQL connection string.</param>
    /// <returns>The updated service collection.</returns>
    internal static IServiceCollection AddDataModel(this IServiceCollection services, string connectionString, DatabaseOptions databaseOptions)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        services.AddSingleton(Options.Create(databaseOptions));
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString, cfg =>
            {
                cfg.EnableRetryOnFailure();
                cfg.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            options.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);
            options.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);
        });

        return services;
    }
}
