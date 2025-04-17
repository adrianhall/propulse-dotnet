using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProPulse.DataModel.Initializer.Interfaces;
using ProPulse.DataModel.Initializer.Services;
using ProPulse.DataModel.Migrations;
using Serilog;

namespace ProPulse.DataModel.Initializer;

/// <summary>
/// The entry point for the database initializer application.
/// </summary>
internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    public static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddEnvironmentVariables()
                      .AddCommandLine(args);
            })
            .UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IDatabaseMigratorService, DatabaseMigratorService>();
                services.AddSingleton<IDatabaseInitializerService, DatabaseInitializerService>();
            })
            .Build();

        using (host)
        {
            await host.Services.GetRequiredService<IDatabaseInitializerService>().RunAsync();
        }
    }
}
