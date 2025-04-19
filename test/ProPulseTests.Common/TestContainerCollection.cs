using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ProPulseTests.Common;

/// <summary>
/// xUnit test collection definition for serializing tests that use test containers.
/// </summary>
[CollectionDefinition("TestContainerCollection")]
[ExcludeFromCodeCoverage]
public sealed class TestContainerCollection
{
    // This class has no code, and is never created.
    // Its purpose is just to be the place to apply [CollectionDefinition].
}
