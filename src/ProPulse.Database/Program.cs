using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProPulse.Database;

// Parse command line args to check for reset flag
bool resetDatabase = args.Contains("--reset");

// Build configuration from environment variables and appsettings.json
IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

// Get connection string from configuration or environment variables
string connectionString = configuration.GetConnectionString("DefaultConnection")
    ?? configuration["Database:ConnectionString"]
    ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
    ?? throw new InvalidOperationException("No database connection string provided.");

// Set up dependency injection
IServiceProvider serviceProvider = new ServiceCollection()
    .AddSingleton<IConfiguration>(configuration)
    .AddLogging(builder =>
    {
        builder.AddConsole();
        builder.SetMinimumLevel(LogLevel.Information);
        builder.AddConfiguration(configuration.GetSection("Logging"));
    })
    .BuildServiceProvider();

// Get logger
ILogger<MigrationRunner> logger = serviceProvider.GetRequiredService<ILoggerFactory>()
    .CreateLogger<MigrationRunner>();

// Run migrations (with optional reset)
MigrationRunner runner = new(connectionString, logger);

// Debug - let's take a look at the embedded script resources.
runner.LogEmbeddedScriptResources();

bool success;

// If reset flag is present, run the reset script first
if (resetDatabase)
{
    logger.LogWarning("Reset flag detected. Executing database reset before migrations...");

    success = runner.ResetDatabase();
    if (!success)
    {
        logger.LogError("Database reset failed.");
        return 1;
    }

    logger.LogInformation("Database reset completed successfully. Proceeding with migrations...");
}

// Run migrations
success = runner.RunMigrations();
if (!success)
{
    logger.LogError("Database migration failed.");
}
return success ? 0 : 1;

