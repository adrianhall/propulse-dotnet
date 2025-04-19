using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using FluentAssertions.Web;

namespace ProPulseTests.Common;

/// <summary>
/// Provides extensions for <see cref="HttpResponseMessageAssertions"/> to validate date/time headers.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HttpResponseMessageDateHeaderAssertions
{
    /// <summary>
    /// Asserts that the response contains the specified header, that it has exactly one value, and that value parses exactly with the given format.
    /// </summary>
    /// <param name="assertions">The response assertions.</param>
    /// <param name="headerName">The header name to check.</param>
    /// <param name="format">The expected date format (e.g., "R" for RFC1123, or a custom format).</param>
    /// <returns>An <see cref="AndConstraint{DateTimeOffsetAssertions}"/> for further assertions.</returns>
    public static AndConstraint<DateTimeOffsetAssertions> HaveDateHeader(this HttpResponseMessageAssertions assertions, string headerName, string format)
    {
        HttpResponseMessage response = assertions.Subject;
        string? value = null;
        bool found = false;

        AssertionChain chain = assertions.CurrentAssertionChain;

        // Try to get from response headers
        if (response.Headers.TryGetValues(headerName, out IEnumerable<string>? values))
        {
            value = values.SingleOrDefault();
            found = value != null;
        }

        // Try to get from content headers if not found
        if (!found && response.Content?.Headers.TryGetValues(headerName, out IEnumerable<string>? contentValues) == true)
        {
            value = contentValues.SingleOrDefault();
            found = value != null;
        }

        chain
            .ForCondition(found)
            .FailWith("Expected header '{0}' to exist in the response, but it was not found.", headerName);

        chain
            .ForCondition(value != null)
            .FailWith("Expected header '{0}' to have a value, but it was not found.", headerName);

        chain
            .ForCondition(!string.IsNullOrEmpty(value))
            .FailWith("Expected header '{0}' to have a non-empty value, but it was empty.", headerName);

        bool parsed = DateTimeOffset.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result);

        chain
            .ForCondition(parsed)
            .FailWith("Expected header '{0}' to be a valid date in format '{1}', but found '{2}'.", headerName, format, value);

        return new AndConstraint<DateTimeOffsetAssertions>(new DateTimeOffsetAssertions(result, chain));
    }
}
