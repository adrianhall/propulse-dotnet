using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProPulse.Core.Options;

namespace ProPulse.DataModel.Tests;

/// <summary>
/// Unit tests for <see cref="ServiceCollectionExtensions"/>.
/// </summary>
public sealed class ServiceCollectionExtensionsTests
{
    /// <summary>
    /// Tests AddDataModel with configuration and verifies the DbContext and 
    /// options are registered.
    /// </summary>
    [Fact]
    public void AddDataModel_WithConfiguration_RegistersDbContextAndOptions()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"ConnectionStrings:DefaultConnection", "Host=localhost;Database=testdb;Username=test;Password=test;"},
            {"Database:Options:EnableDetailedErrors", "true"},
            {"Database:Options:EnableSensitiveDataLogging", "true"}
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        IServiceCollection services = new ServiceCollection();

        services.AddDataModel(configuration);
        IServiceProvider provider = services.BuildServiceProvider();

        provider.GetService<AppDbContext>().Should().NotBeNull();
        provider.GetService<IOptions<DatabaseOptions>>().Should().NotBeNull();
        provider.GetService<IOptions<DatabaseOptions>>()!.Value.EnableDetailedErrors.Should().BeTrue();
        provider.GetService<IOptions<DatabaseOptions>>()!.Value.EnableSensitiveDataLogging.Should().BeTrue();
    }

    /// <summary>
    /// Tests AddDataModel with configuration that does not include a connection string.
    /// </summary>
    [Fact]
    public void AddDataModel_WithConfiguration_NoConnectionString_ThrowsArgumentException()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Database:Options:EnableDetailedErrors", "true"},
            {"Database:Options:EnableSensitiveDataLogging", "true"}
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        IServiceCollection services = new ServiceCollection();

        Action act = () => services.AddDataModel(configuration);

        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Tests AddDataModel with a connection string only.
    /// </summary>
    [Fact]
    public void AddDataModel_WithConnectionString_RegistersDbContextAndDefaultOptions()
    {
        const string connectionString = "Host=localhost;Database=testdb;Username=test;Password=test;";
        IServiceCollection services = new ServiceCollection();
        services.AddDataModel(connectionString);
        IServiceProvider provider = services.BuildServiceProvider();

        provider.GetService<AppDbContext>().Should().NotBeNull();
        provider.GetService<IOptions<DatabaseOptions>>().Should().NotBeNull();
        provider.GetService<IOptions<DatabaseOptions>>()!.Value.EnableDetailedErrors.Should().BeFalse();
        provider.GetService<IOptions<DatabaseOptions>>()!.Value.EnableSensitiveDataLogging.Should().BeFalse();
    }

    /// <summary>
    /// Tests AddDataModel with a connection string and explicit options.
    /// </summary>
    [Fact]
    public void AddDataModel_WithConnectionStringAndOptions_RegistersDbContextAndOptions()
    {
        const string connectionString = "Host=localhost;Database=testdb;Username=test;Password=test;";
        var options = new DatabaseOptions { EnableDetailedErrors = true, EnableSensitiveDataLogging = true };
        IServiceCollection services = new ServiceCollection();
        services.AddDataModel(connectionString, options);
        IServiceProvider provider = services.BuildServiceProvider();

        provider.GetService<AppDbContext>().Should().NotBeNull();
        provider.GetService<IOptions<DatabaseOptions>>().Should().NotBeNull();
        provider.GetService<IOptions<DatabaseOptions>>()!.Value.EnableDetailedErrors.Should().BeTrue();
        provider.GetService<IOptions<DatabaseOptions>>()!.Value.EnableSensitiveDataLogging.Should().BeTrue();
    }

    /// <summary>
    /// Tests AddDataModel throws when the connection string is empty or null.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AddDataModel_ThrowsOnEmptyConnectionString(string? connectionString)
    {
        IServiceCollection services = new ServiceCollection();
        Action act = () => services.AddDataModel(connectionString!);
        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// Tests AddDataModel with a connection string but no database options section.
    /// </summary>
    [Fact]
    public void AddDataModel_WithConnectionString_NoDatabaseOptions_UsesDefaults()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"ConnectionStrings:DefaultConnection", "Host=localhost;Database=testdb;Username=test;Password=test;"}
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        IServiceCollection services = new ServiceCollection();

        services.AddDataModel(configuration);
        IServiceProvider provider = services.BuildServiceProvider();

        provider.GetService<AppDbContext>().Should().NotBeNull();
        provider.GetService<IOptions<DatabaseOptions>>().Should().NotBeNull();
        provider.GetService<IOptions<DatabaseOptions>>()!.Value.EnableDetailedErrors.Should().BeFalse();
        provider.GetService<IOptions<DatabaseOptions>>()!.Value.EnableSensitiveDataLogging.Should().BeFalse();
    }

    /// <summary>
    /// Tests AddDataModel with invalid option values in the configuration.
    /// </summary>
    [Fact]
    public void AddDataModel_WithInvalidOptionValues_UsesDefaults()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"ConnectionStrings:DefaultConnection", "Host=localhost;Database=testdb;Username=test;Password=test;"},
            {"Database:Options:EnableDetailedErrors", "notabool"},
            {"Database:Options:EnableSensitiveDataLogging", "notabool"}
        };
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        IServiceCollection services = new ServiceCollection();

        Action act = () => services.AddDataModel(configuration);

        act.Should().Throw<InvalidOperationException>();
    }
}
