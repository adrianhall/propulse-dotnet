using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;

namespace ProPulse.DataModel.Migrations.Tests;

[ExcludeFromCodeCoverage]
public class DatabaseMigratorIntegrationTests(PostgreSqlContainerFixture fixture) : IClassFixture<PostgreSqlContainerFixture>
{
    /// <summary>
    /// Verifies that the initial migration creates the articles table and triggers.
    /// </summary>
    [Fact]
    public async Task Should_Apply_Initial_Migration_And_Create_Articles_Table_With_Triggers()
    {
        await fixture.EnsureInitializedAsync(withMigrations: false);
        DatabaseMigrator migrator = new() { Logger = NullLogger.Instance };
        migrator.ApplyMigrations(fixture.ConnectionString); 

        // Assert: 001-propulse-init.sql should be in applied scripts
        migrator.AppliedScripts.Should().Contain(s => s.Contains("001-propulse-init.sql"));

        // Let's get a list of the tables that are in the database
        var tables = await fixture.ExecuteQueryAsync("SELECT table_name FROM information_schema.tables WHERE table_schema = 'propulse';");

        tables.Should().NotBeNullOrEmpty()
            .And.Contain(t => t.ContainsKeyWithValue("table_name", "Articles"));

        // Check triggers exist on Articles table
        var triggers = await fixture.ExecuteQueryAsync("SELECT * FROM information_schema.triggers;");
        triggers.Should().NotBeNullOrEmpty()
            .And.Contain(t => IsTriggerDefinition(t, "INSERT", "propulse", "Articles"))
            .And.Contain(t => IsTriggerDefinition(t, "UPDATE", "propulse", "Articles"));
    }

    /// <summary>
    /// Verifies that only valid scripts are applied from the test assembly with resource filtering.
    /// </summary>
    [Fact]
    public async Task Should_Apply_Scripts_From_Test_Assembly_With_Resource_Filtering()
    {
        await fixture.EnsureInitializedAsync(withMigrations: false);
        Assembly testAssembly = GetType().Assembly;
        string resourcePrefix = testAssembly.GetName().Name + ".Scripts.";
        DatabaseMigrator migrator = new(testAssembly, resourcePrefix) { Logger = NullLogger.Instance };

        // Act
        migrator.ApplyMigrations(fixture.ConnectionString);

        // Assert: Only the valid SQL script in Scripts/ should be applied
        migrator.AppliedScripts.Should().Contain(s => s.Contains("002-test.sql"));
        migrator.AppliedScripts.Should().NotContain(s => s.Contains("should_not_apply.sql"));
        migrator.AppliedScripts.Should().NotContain(s => s.Contains("readme.txt"));

        // Check that the test_embedded_table exists and the other table does not.
        var tables = await fixture.ExecuteQueryAsync("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';");
        tables.Should()
            .Contain(t => t.ContainsKeyWithValue("table_name", "test_embedded_table"))
            .And.NotContain(t => t.ContainsKeyWithValue("table_name", "should_not_be_created"));
    }

    /// <summary>
    /// Verifies that a bad script causes an exception and is not applied.
    /// </summary>
    [Fact]
    public async Task Should_Throw_And_Not_Apply_Bad_Scripts_From_Custom_ResourcePrefix()
    {
        // Arrange
        await fixture.EnsureInitializedAsync(withMigrations: false);
        Assembly testAssembly = GetType().Assembly;
        string resourcePrefix = testAssembly.GetName().Name + ".BadScripts.";
        DatabaseMigrator migrator = new(testAssembly, resourcePrefix) { Logger = NullLogger.Instance };

        // Act
        Action act = () => migrator.ApplyMigrations(fixture.ConnectionString);

        // Assert
        act.Should().Throw<DatabaseMigrationException>();
        migrator.AppliedScripts.Should().NotContain(s => s.Contains("003-bad.sql"));
    }

    /// <summary>
    /// Checks if a row represents a trigger definition for a given table and change type.
    /// </summary>
    private static bool IsTriggerDefinition(Dictionary<string, object> rowData, string changeType, string schema, string tableName)
        => rowData.ContainsKeyWithValue("event_manipulation", changeType)
            && rowData.ContainsKeyWithValue("event_object_schema", schema)
            && rowData.ContainsKeyWithValue("event_object_table", tableName);
}
