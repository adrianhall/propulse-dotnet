namespace ProPulse.Core.Options;

/// <summary>
/// A model class representing the database options for the application.
/// </summary>
public class DatabaseOptions
{
    /// <summary>
    /// If true, detailed errors will be emitted.
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;

    /// <summary>
    /// If true, sensitive data (such as parameter values) will be logged.
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;
}