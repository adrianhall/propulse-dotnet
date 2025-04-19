using System.Diagnostics;
using FluentAssertions;
using ProPulseTests.Common;
using Xunit;

namespace ProPulse.DataModel.Initializer.Tests;

/// <summary>
/// End-to-end tests for the ProPulse.DataModel.Initializer console application.
/// </summary>
[Collection("TestContainerCollection")]
public sealed class InitializerEndToEndTests(PostgreSqlContainerFixture fixture) : IClassFixture<PostgreSqlContainerFixture>
{
    /// <summary>
    /// The location of the Initializer executable.
    /// </summary>
    private static readonly string ExecutablePath = typeof(InitializerEndToEndTests).Assembly.Location
        .Replace(".Tests.dll", ".dll")
        .Replace("test\\ProPulse.DataModel.Initializer.Tests", "src/ProPulse.DataModel.Initializer");

    /// <summary>
    /// Verifies that running the initializer with a valid connection string migrates the database successfully.
    /// </summary>
    [Fact]
    public async Task Initializer_MainLink_RunsMigrations_WhenConnectionStringProvided()
    {
        await fixture.EnsureInitializedAsync(withMigrations: false);

        // Act: Call Program.Main directly with the connection string as a command-line argument
        string[] args = [ "/ConnectionStrings:DefaultConnection", fixture.ConnectionString];
        await Program.Main(args);

        // Check that the DbUp journal table exists
        var publicTables = await fixture.ExecuteQueryAsync("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';");
        publicTables.Should().NotBeNullOrEmpty()
            .And.Contain(t => t.ContainsKeyWithValue("table_name", "DbUpSchemaVersion"));

        // Let's get a list of the tables that are in the database
        var propulseTables = await fixture.ExecuteQueryAsync("SELECT table_name FROM information_schema.tables WHERE table_schema = 'propulse';");
        propulseTables.Should().NotBeNullOrEmpty()
            .And.Contain(t => t.ContainsKeyWithValue("table_name", "Articles"));
    }

    /// <summary>
    /// Verifies that running the initializer with a valid connection string migrates the database successfully.
    /// </summary>
    [Fact]
    public async Task Initializer_RunsMigrations_WhenConnectionStringProvided()
    {
        await fixture.EnsureInitializedAsync(withMigrations: false);
        var startInfo = new ProcessStartInfo("dotnet", $"{ExecutablePath}")
        {
            Environment = { ["ConnectionStrings__DefaultConnection"] = fixture.ConnectionString },
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        // Act
        using Process process = Process.Start(startInfo)!;
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        // Assert
        process.ExitCode.Should().Be(0, error + output);
        output.Should().Contain("Database migrations applied");
    }

    /// <summary>
    /// Verifies that running the initializer without a connection string fails with an error.
    /// </summary>
    [Fact]
    public async Task Initializer_Throws_WhenConnectionStringMissing()
    {
        // Arrange
        var startInfo = new ProcessStartInfo("dotnet", $"{ExecutablePath}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        // Act
        using Process process = Process.Start(startInfo)!;
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        // Assert
        process.ExitCode.Should().NotBe(0);
        error.Should().Contain("Connection string not found");
    }
}
