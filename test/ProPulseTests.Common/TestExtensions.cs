using System.Diagnostics.CodeAnalysis;

namespace ProPulseTests.Common;

/// <summary>
/// A set of extensions methods to make the code easier to read.
/// </summary>
[ExcludeFromCodeCoverage]
public static class TestExtensions
{
    /// <summary>
    /// Checks if a dictionary contains a specific key with a specific value.
    /// </summary>
    public static bool ContainsKeyWithValue(this Dictionary<string, object> rowData, string key, string value)
        => rowData.ContainsKey(key) && rowData[key]?.ToString() == value;
}