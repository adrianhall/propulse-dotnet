using System.Reflection;
using DbUp.Engine;
using Microsoft.Extensions.Logging.Abstractions;

namespace ProPulse.DataModel.Migrations.Tests;

/// <summary>
/// Unit tests for <see cref="DatabaseMigrator"/> and <see cref="DatabaseMigrationException"/> that do not 
/// require a database connection.
/// </summary>
[ExcludeFromCodeCoverage]
public class DatabaseMigratorUnitTests
{
    /// <summary>
    /// Verifies the default constructor sets the expected assembly and resource prefix.
    /// </summary>
    [Fact]
    public void Constructor_Defaults_To_ExecutingAssembly_And_DefaultPrefix()
    {
        Assembly expectedAssembly = typeof(DatabaseMigrator).Assembly;
        const string expectedPrefix = "ProPulse.DataModel.Migrations.Scripts.";

        DatabaseMigrator migrator = new(null, null);

        migrator.ContainingAssembly.Should().BeSameAs(expectedAssembly);
        migrator.ResourcePrefix.Should().Be(expectedPrefix);
    }

    /// <summary>
    /// Verifies the constructor sets the expected assembly and default prefix.
    /// </summary>
    [Fact]
    public void Constructor_ExplicitAssembly_DefaultPrefix()
    {
        Assembly expectedAssembly = typeof(DatabaseMigratorUnitTests).Assembly;
        const string expectedPrefix = "ProPulse.DataModel.Migrations.Tests.Scripts.";

        DatabaseMigrator migrator = new(expectedAssembly, null);

        migrator.ContainingAssembly.Should().BeSameAs(expectedAssembly);
        migrator.ResourcePrefix.Should().Be(expectedPrefix);
    }

    /// <summary>
    /// Verifies the constructor sets the expected assembly and explicit prefix.
    /// </summary>
    [Fact]
    public void Constructor_ExplicitAssembly_ExplicitPrefix()
    {
        Assembly expectedAssembly = typeof(DatabaseMigratorUnitTests).Assembly;
        string customPrefix = "Custom.Prefix.";

        DatabaseMigrator migrator = new(expectedAssembly, customPrefix);

        migrator.ContainingAssembly.Should().BeSameAs(expectedAssembly);
        migrator.ResourcePrefix.Should().Be(customPrefix);
    }

    /// <summary>
    /// Verifies LogAndThrowErrors does nothing when the result is successful.
    /// </summary>
    [Fact]
    public void LogAndThrowErrors_Successful_DoesNothing()
    {
        DatabaseMigrator migrator = new(null, null) { Logger = NullLogger.Instance };
        DatabaseUpgradeResult result = new([], true, null, null);
        migrator.LogAndThrowErrors(result);
    }

    /// <summary>
    /// Verifies LogAndThrowErrors throws when the result is unsuccessful and ErrorScript is not null.
    /// </summary>
    [Fact]
    public void LogAndThrowErrors_Unsuccessful_ThrowsAndLogs()
    {
        DatabaseMigrator migrator = new(null, null) { Logger = NullLogger.Instance };
        SqlScript errorScript = new("bad.sql", "bad sql");
        Exception error = new("Test migration error");
        DatabaseUpgradeResult result = new([errorScript], false, error, errorScript);

        Action act = () => migrator.LogAndThrowErrors(result);

        act.Should().Throw<DatabaseMigrationException>();
    }

    /// <summary>
    /// Verifies LogAndThrowErrors throws when the result is unsuccessful and ErrorScript is null.
    /// </summary>
    [Fact]
    public void LogAndThrowErrors_Unsuccessful_NullErrorScript_ThrowsAndLogs()
    {
        DatabaseMigrator migrator = new(null, null) { Logger = NullLogger.Instance };
        Exception error = new("Test migration error");
        SqlScript errorScript = new("bad.sql", "bad sql");
        DatabaseUpgradeResult result = new([errorScript], false, error, null);

        Action act = () => migrator.LogAndThrowErrors(result);

        act.Should().Throw<DatabaseMigrationException>();
    }

    /// <summary>
    /// Verifies LogAndThrowErrors throws when the result is unsuccessful and Error is null.
    /// </summary>
    [Fact]
    public void LogAndThrowErrors_Unsuccessful_NullError_ThrowsAndLogs()
    {
        DatabaseMigrator migrator = new(null, null) { Logger = NullLogger.Instance };
        SqlScript errorScript = new("bad.sql", "bad sql");
        DatabaseUpgradeResult result = new([errorScript], false, null, errorScript);

        Action act = () => migrator.LogAndThrowErrors(result);

        act.Should().Throw<DatabaseMigrationException>();
    }

    /// <summary>
    /// Verifies the default constructor of DatabaseMigrationException.
    /// </summary>
    [Fact]
    public void DatabaseMigrationException_DefaultConstructor_CanBeConstructed()
    {
        DatabaseMigrationException ex = new();
        ex.Should().BeOfType<DatabaseMigrationException>();
    }

    /// <summary>
    /// Verifies the string constructor of DatabaseMigrationException.
    /// </summary>
    [Fact]
    public void DatabaseMigrationException_StringConstructor_CanBeConstructed()
    {
        DatabaseMigrationException ex = new("test");
        ex.Message.Should().Be("test");
    }

    /// <summary>
    /// Verifies the string and exception constructor of DatabaseMigrationException.
    /// </summary>
    [Fact]
    public void DatabaseMigrationException_StringAndExceptionConstructor_CanBeConstructed()
    {
        Exception innerEx = new("inner exception");
        DatabaseMigrationException ex = new("test", innerEx);
        ex.Message.Should().Be("test");
        ex.InnerException.Should().BeSameAs(innerEx);
    }

    /// <summary>
    /// Verifies the string and null exception constructor of DatabaseMigrationException.
    /// </summary>
    [Fact]
    public void DatabaseMigrationException_NullInnerException_CanBeConstructed()
    {
        DatabaseMigrationException ex = new("test", null);
        ex.Message.Should().Be("test");
        ex.InnerException.Should().BeNull();
    }
}
