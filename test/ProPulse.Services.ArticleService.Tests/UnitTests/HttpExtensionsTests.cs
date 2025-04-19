using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using ProPulse.Services.ArticleService.Extensions;

namespace ProPulse.Services.ArticleService.Tests.UnitTests;

/// <summary>
/// Unit tests for <see cref="HttpExtensions"/>.
/// </summary>
public class HttpExtensionsTests
{
    [Fact]
    public void GetRequestHeader_ReturnsHeaderValue_WhenSingleValuePresent()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Test-Header"] = "value1";
        var modelState = new ModelStateDictionary();

        // Act
        string? result = context.GetRequestHeader("X-Test-Header", modelState);

        // Assert
        result.Should().Be("value1");
        modelState.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GetRequestHeader_AddsModelError_WhenMultipleValuesPresent()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Test-Header"] = new StringValues([ "value1", "value2" ]);
        var modelState = new ModelStateDictionary();

        // Act
        string? result = context.GetRequestHeader("X-Test-Header", modelState);

        // Assert
        result.Should().Be("value1");
        modelState.IsValid.Should().BeFalse();
        modelState["X-Test-Header"]?.Errors.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Verifies that GetRequestHeader returns null and does not add a model error when the header is missing.
    /// </summary>
    [Fact]
    public void GetRequestHeader_ReturnsNull_WhenHeaderNotPresent()
    {
        // Arrange
        DefaultHttpContext context = new();
        ModelStateDictionary modelState = new();

        // Act
        string? result = context.GetRequestHeader("Missing-Header", modelState);

        // Assert
        result.Should().BeNull();
        modelState.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void ValidateBooleanHeader_InvokesAction_WhenValidBoolean(string value, bool expected)
    {
        // Arrange
        var actionContext = CreateActionExecutingContextWithHeader("X-Bool", value);
        bool? parsed = null;

        // Act
        actionContext.ValidateBooleanHeader("X-Bool", b => parsed = b);

        // Assert
        parsed.Should().Be(expected);
        actionContext.ModelState.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateBooleanHeader_AddsModelError_WhenInvalidBoolean()
    {
        // Arrange
        var actionContext = CreateActionExecutingContextWithHeader("X-Bool", "notabool");
        bool? parsed = null;

        // Act
        actionContext.ValidateBooleanHeader("X-Bool", b => parsed = b);

        // Assert
        parsed.Should().BeNull();
        actionContext.ModelState.IsValid.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that ValidateBooleanHeader does not invoke the action and does not add a model error when the header is missing.
    /// </summary>
    [Fact]
    public void ValidateBooleanHeader_DoesNothing_WhenHeaderNotPresent()
    {
        // Arrange
        ActionExecutingContext actionContext = CreateActionExecutingContextWithHeader("Other-Header", "true");
        bool actionInvoked = false;

        // Act
        actionContext.ValidateBooleanHeader("Missing-Header", b => actionInvoked = true);

        // Assert
        actionInvoked.Should().BeFalse();
        actionContext.ModelState.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("*")]
    [InlineData("\"valid123\"")]
    public void ValidateETagHeader_InvokesAction_WhenValidETag(string etag)
    {
        // Arrange
        var actionContext = CreateActionExecutingContextWithHeader("If-Match", etag);
        string? parsed = null;

        // Act
        actionContext.ValidateETagHeader("If-Match", s => parsed = s);

        // Assert
        parsed.Should().Be(etag);
        actionContext.ModelState.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("invalid-etag")]
    [InlineData("\"invalid-etag")]
    [InlineData("invalid-etag\"")]
    [InlineData("\"\"")]
    [InlineData("\"*\"")]
    public void ValidateETagHeader_AddsModelError_WhenInvalidETag(string etag)
    {
        // Arrange
        var actionContext = CreateActionExecutingContextWithHeader("If-Match", etag);
        string? parsed = null;

        // Act
        actionContext.ValidateETagHeader("If-Match", s => parsed = s);

        // Assert
        parsed.Should().BeNull();
        actionContext.ModelState.IsValid.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that ValidateETagHeader does not invoke the action and does not add a model error when the header is missing.
    /// </summary>
    [Fact]
    public void ValidateETagHeader_DoesNothing_WhenHeaderNotPresent()
    {
        // Arrange
        ActionExecutingContext actionContext = CreateActionExecutingContextWithHeader("Other-Header", "\"etag\"");
        bool actionInvoked = false;

        // Act
        actionContext.ValidateETagHeader("Missing-Header", s => actionInvoked = true);

        // Assert
        actionInvoked.Should().BeFalse();
        actionContext.ModelState.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("If-Modified-Since", "Wed, 21 Oct 2015 07:28:13 GMT")]
    [InlineData("If-Modified-Since-Timestamp", "2015-10-21T07:28:13.012345Z")]
    public void ValidateDateHeader_InvokesAction_WhenValidDate(string headerName, string headerValue)
    {
        // Arrange
        DateTimeOffset testDate = DateTimeOffset.Parse(headerValue);
        var actionContext = CreateActionExecutingContextWithHeader(headerName, headerValue);
        DateTimeOffset? parsed = null;

        // Act
        actionContext.ValidateDateHeader("If-Modified-Since", d => parsed = d);

        // Assert
        parsed.Should().NotBeNull().And.BeCloseTo(testDate, TimeSpan.FromSeconds(1));
        actionContext.ModelState.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("If-Modified-Since", "notadate")]
    [InlineData("If-Modified-Since", "2025-04-18T12:34:56Z")] // ISO8601 in RFC1123 header
    [InlineData("If-Modified-Since", "Fri, 18 Apr 25 12:34:56 GMT")] // RFC850 format
    [InlineData("If-Modified-Since", "18 Apr 2025 12:34:56 GMT")] // asctime format
    [InlineData("If-Modified-Since-Timestamp", "Fri, 18 Apr 2025 12:34:56 GMT")] // RFC1123 in Timestamp header
    [InlineData("If-Modified-Since", "April 18, 2025 12:34:56")]
    [InlineData("If-Modified-Since", "2025/04/18 12:34:56")]
    public void ValidateDateHeader_AddsModelError_WhenInvalidRfc1123Date(string headerName, string headerValue)
    {
        // Arrange
        var actionContext = CreateActionExecutingContextWithHeader(headerName, headerValue);
        DateTimeOffset? parsed = null;

        // Act
        actionContext.ValidateDateHeader("If-Modified-Since", d => parsed = d);

        // Assert
        parsed.Should().BeNull();
        actionContext.ModelState.IsValid.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that ValidateDateHeader does not invoke the action and does not add a model error when the header is missing.
    /// </summary>
    [Fact]
    public void ValidateDateHeader_DoesNothing_WhenHeaderNotPresent()
    {
        // Arrange
        ActionExecutingContext actionContext = CreateActionExecutingContextWithHeader("Other-Header", "Wed, 21 Oct 2015 07:28:13 GMT");
        bool actionInvoked = false;

        // Act
        actionContext.ValidateDateHeader("Missing-Header", d => actionInvoked = true);

        // Assert
        actionInvoked.Should().BeFalse();
        actionContext.ModelState.IsValid.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that ValidateDateHeader adds a model error when both date headers are present.
    /// </summary>
    [Fact]
    public void ValidateDateHeader_AddsModelError_WhenBothHeadersPresent()
    {
        // Arrange
        string headerName = "If-Modified-Since";
        string timestampHeaderName = headerName + "-Timestamp";
        ActionExecutingContext actionContext = CreateActionExecutingContextWithHeader(headerName, "Wed, 21 Oct 2015 07:28:13 GMT");
        actionContext.HttpContext.Request.Headers[timestampHeaderName] = "2015-10-21T07:28:13.012345Z";
        bool actionInvoked = false;

        // Act
        actionContext.ValidateDateHeader(headerName, d => actionInvoked = true);

        // Assert
        actionInvoked.Should().BeFalse();
        actionContext.ModelState.IsValid.Should().BeFalse();
        actionContext.ModelState[headerName]?.Errors.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Verifies that ValidateDateHeader adds a model error when the header value is present but not a valid date.
    /// </summary>
    [Fact]
    public void ValidateDateHeader_AddsModelError_WhenHeaderValueInvalid()
    {
        // Arrange
        string headerName = "If-Modified-Since";
        ActionExecutingContext actionContext = CreateActionExecutingContextWithHeader(headerName, "not-a-date");
        bool actionInvoked = false;

        // Act
        actionContext.ValidateDateHeader(headerName, d => actionInvoked = true);

        // Assert
        actionInvoked.Should().BeFalse();
        actionContext.ModelState.IsValid.Should().BeFalse();
        actionContext.ModelState[headerName]?.Errors.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Verifies that ValidateDateHeader does not invoke the action and does not add a model error when neither header is present.
    /// </summary>
    [Fact]
    public void ValidateDateHeader_DoesNothing_WhenNeitherHeaderPresent()
    {
        // Arrange
        string headerName = "If-Modified-Since";
        ActionExecutingContext actionContext = CreateActionExecutingContextWithHeader("Other-Header", "value");
        bool actionInvoked = false;

        // Act
        actionContext.ValidateDateHeader(headerName, d => actionInvoked = true);

        // Assert
        actionInvoked.Should().BeFalse();
        actionContext.ModelState.IsValid.Should().BeTrue();
    }

    private static ActionExecutingContext CreateActionExecutingContextWithHeader(string headerName, string headerValue)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[headerName] = headerValue;
        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(), new ModelStateDictionary());
        return new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), new object());
    }
}
