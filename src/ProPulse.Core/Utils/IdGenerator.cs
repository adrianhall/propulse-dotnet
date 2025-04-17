namespace ProPulse.Core.Utils;

/// <summary>
/// A utility class for generating unique identifiers.
/// </summary>
public static class IdGenerator
{
    /// <summary>
    /// Creates a new unique identifier (ULID) string.
    /// </summary>
    /// <remarks>
    /// This method uses the Ulid library to generate a ULID, which is a 128-bit identifier that is 
    /// lexicographically sortable and time-based.
    /// </remarks>
    /// <returns>a unique identifier as a string.</returns>
    public static string NewId()
    {
        return Ulid.NewUlid().ToString();
    }
}