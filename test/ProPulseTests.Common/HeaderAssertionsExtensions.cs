using FluentAssertions;
using FluentAssertions.Collections;
using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace ProPulseTests.Common;

/// <summary>
/// Provides FluentAssertions extensions for asserting HTTP headers.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HeaderAssertionsExtensions
{
    /// <summary>
    /// Begins assertion on a header in the header dictionary.
    /// </summary>
    /// <param name="headers">The header dictionary.</param>
    /// <param name="headerName">The header name to assert.</param>
    /// <returns>A <see cref="HeaderValueAssertions"/> for further assertions.</returns>
    public static HeaderValueAssertions HaveHeader(this GenericDictionaryAssertions<IDictionary<string, StringValues>, string, StringValues> assertions, string headerName)
    {
        assertions.Subject.Should().ContainKey(headerName, $"Expected header '{headerName}' to be present.");
        return new HeaderValueAssertions(assertions.Subject, headerName);
    }
}

/// <summary>
/// Provides assertions for a specific header value.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class HeaderValueAssertions(IDictionary<string, StringValues> headers, string headerName) 
{
    /// <summary>
    /// Asserts that the header has the specified value.
    /// </summary>
    /// <param name="expectedValue">The expected header value.</param>
    /// <returns>This assertion object for chaining.</returns>
    public HeaderValueAssertions WithValue(string expectedValue)
    {
        headers[headerName].Should()
            .NotBeNullOrEmpty($"Expected header '{headerName}' to have a value but it was not found.")
            .And.ContainSingle(expectedValue, $"Expected header '{headerName}' to have value '{expectedValue}' but found '{{_headers[_headerName]}}'.");
        return this;
    }
}
