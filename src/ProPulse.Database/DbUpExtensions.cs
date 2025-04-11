using DbUp.Builder;
using DbUp.Engine;
using DbUp.Support;

namespace ProPulse.Database;

/// <summary>
/// A set of extension methods for helping to configure the DbUp upgrade engine.
/// </summary>
public static class DbUpExtensions
{
    /// <summary>
    /// Configures the upgrade engine to include all database migration scripts for the ProPulse application.
    /// </summary>
    /// <param name="builder">The upgrade engine builder to configure.</param>
    /// <returns>The configured upgrade engine builder.</returns>
    public static UpgradeEngineBuilder WithProPulseScripts(this UpgradeEngineBuilder builder)
    {
        return builder
            .WithIdentitySchemaScripts()
            .WithProPulseSchemaScripts()
            .WithDataScripts();
    }

    /// <summary>
    /// Configures the upgrade engine to include identity schema migration scripts, which will execute first.
    /// </summary>
    /// <param name="builder">The upgrade engine builder to configure.</param>
    /// <returns>The configured upgrade engine builder.</returns>
    public static UpgradeEngineBuilder WithIdentitySchemaScripts(this UpgradeEngineBuilder builder)
    {
        return builder.WithScriptsEmbeddedInAssembly(
            typeof(MigrationRunner).Assembly,
            script => script.StartsWith("ProPulse.Database.Scripts.Schema.identity"),
            new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 1 }
        );
    }

    /// <summary>
    /// Configures the upgrade engine to include propulse schema migration scripts, which will execute after identity schema scripts.
    /// </summary>
    /// <param name="builder">The upgrade engine builder to configure.</param>
    /// <returns>The configured upgrade engine builder.</returns>
    public static UpgradeEngineBuilder WithProPulseSchemaScripts(this UpgradeEngineBuilder builder)
    {
        return builder.WithScriptsEmbeddedInAssembly(
            typeof(MigrationRunner).Assembly,
            script => script.StartsWith("ProPulse.Database.Scripts.Schema.propulse"),
            new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 2 }
        );
    }

    /// <summary>
    /// Configures the upgrade engine to include data migration scripts, which will execute after all schema scripts.
    /// </summary>
    /// <param name="builder">The upgrade engine builder to configure.</param>
    /// <returns>The configured upgrade engine builder.</returns>
    public static UpgradeEngineBuilder WithDataScripts(this UpgradeEngineBuilder builder)
    {
        return builder.WithScriptsEmbeddedInAssembly(
            typeof(MigrationRunner).Assembly,
            script => script.StartsWith("ProPulse.Database.Scripts.Data"),
            new SqlScriptOptions { ScriptType = ScriptType.RunOnce, RunGroupOrder = 3 }
        );
    }

}
