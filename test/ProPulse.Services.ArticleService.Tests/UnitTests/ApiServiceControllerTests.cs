using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using ProPulse.Core.Entities;
using ProPulse.Services.ArticleService.Controllers;
using ProPulse.Services.ArticleService.ViewModels;

namespace ProPulse.Services.ArticleService.Tests.UnitTests;

/// <summary>
/// Unit tests for <see cref="ApiServiceController"/> abstract class.
/// </summary>
public sealed class ApiServiceControllerTests
{
    private sealed class TestController : ApiServiceController
    {
        /// <summary>
        /// Sets the IfMatch header value for testing.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void SetIfMatch(string? value) => ConditionalHeaderValues.IfMatch = value;

        /// <summary>
        /// Sets the IfUnmodifiedSince header value for testing.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public void SetIfUnmodifiedSince(DateTimeOffset? value) => ConditionalHeaderValues.IfUnmodifiedSince = value;

        /// <summary>
        /// Exposes the protected ConditionalHeaderValues for testing (as object).
        /// </summary>
        public object ExposeConditionalHeaderValues => ConditionalHeaderValues;
        /// <summary>
        /// Exposes the protected IncludeDeletedEntities for testing.
        /// </summary>
        public bool ExposeIncludeDeletedEntities => IncludeDeletedEntities;
        /// <summary>
        /// Gets the IfMatch value for testing.
        /// </summary>
        public string? GetIfMatch() => ConditionalHeaderValues.GetType().GetProperty("IfMatch")?.GetValue(ConditionalHeaderValues) as string;
        /// <summary>
        /// Gets the IfNoneMatch value for testing.
        /// </summary>
        public string? GetIfNoneMatch() => ConditionalHeaderValues.GetType().GetProperty("IfNoneMatch")?.GetValue(ConditionalHeaderValues) as string;
        /// <summary>
        /// Gets the IfModifiedSince value for testing.
        /// </summary>
        public DateTimeOffset? GetIfModifiedSince() => (DateTimeOffset?)ConditionalHeaderValues.GetType().GetProperty("IfModifiedSince")?.GetValue(ConditionalHeaderValues);
        /// <summary>
        /// Gets the IfUnmodifiedSince value for testing.
        /// </summary>
        public DateTimeOffset? GetIfUnmodifiedSince() => (DateTimeOffset?)ConditionalHeaderValues.GetType().GetProperty("IfUnmodifiedSince")?.GetValue(ConditionalHeaderValues);

        public void InvokeOnActionExecuted(ActionExecutedContext context) => OnActionExecuted(context);
        public bool TestEntityIsModified(BaseEntity entity) => EntityIsModified(entity);
        public void SetIfNoneMatch(string? value) => ConditionalHeaderValues.IfNoneMatch = value;
        public void SetIfModifiedSince(DateTimeOffset? value) => ConditionalHeaderValues.IfModifiedSince = value;
    }

    [Fact]
    public void OnActionExecuted_SetsHeaders_WhenResultIsBaseEntityResponseAndStatus200Or201()
    {
        // Arrange
        var controller = new TestController();
        var httpContext = new DefaultHttpContext();
        var entity = new ArticleResponse { Id = Guid.NewGuid(), UpdatedAt = DateTimeOffset.UtcNow, Version = [1,2,3] };
        var result = new ObjectResult(entity) { StatusCode = StatusCodes.Status200OK };
        var context = new ActionExecutedContext(
            new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            [], controller) { Result = result };

        // Act
        controller.InvokeOnActionExecuted(context);

        // Assert
        httpContext.Response.Headers.ETag.Should().NotBeNullOrEmpty();
        httpContext.Response.Headers.LastModified.Should().NotBeNullOrEmpty();
        httpContext.Response.Headers["Last-Modified-Timestamp"].Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Verifies that OnActionExecuted does not set headers when the result is an ObjectResult with a status code other than 200 or 201.
    /// </summary>
    [Fact]
    public void OnActionExecuted_DoesNotSetHeaders_WhenStatusCodeIsNot200Or201()
    {
        // Arrange
        TestController controller = new();
        DefaultHttpContext httpContext = new();
        ArticleResponse entity = new() { Id = Guid.NewGuid(), UpdatedAt = DateTimeOffset.UtcNow, Version = [1, 2, 3] };
        ObjectResult result = new(entity) { StatusCode = StatusCodes.Status404NotFound };
        ActionExecutedContext context = new(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
            [], controller) { Result = result };

        // Act
        controller.InvokeOnActionExecuted(context);

        // Assert
        httpContext.Response.Headers.ETag.Should().BeNullOrEmpty();
        httpContext.Response.Headers.LastModified.Should().BeNullOrEmpty();
        httpContext.Response.Headers.ContainsKey("Last-Modified-Timestamp").Should().BeFalse();
    }

    /// <summary>
    /// Verifies that OnActionExecuted does not set headers when the result is not an ObjectResult.
    /// </summary>
    [Fact]
    public void OnActionExecuted_DoesNotSetHeaders_WhenResultIsNotObjectResult()
    {
        // Arrange
        TestController controller = new();
        DefaultHttpContext httpContext = new();
        ContentResult result = new() { Content = "test content" };
        ActionExecutedContext context = new(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
            [], controller) { Result = result };

        // Act
        controller.InvokeOnActionExecuted(context);

        // Assert
        httpContext.Response.Headers.ETag.Should().BeNullOrEmpty();
        httpContext.Response.Headers.LastModified.Should().BeNullOrEmpty();
        httpContext.Response.Headers.ContainsKey("Last-Modified-Timestamp").Should().BeFalse();
    }

    /// <summary>
    /// Verifies that EntityIsModified returns false when both IfNoneMatch and IfModifiedSince are null.
    /// </summary>
    [Fact]
    public void EntityIsModified_ReturnsFalse_WhenNoConditionalHeadersAreSet()
    {
        // Arrange
        TestController controller = new();
        Article entity = new() { UpdatedAt = DateTimeOffset.UtcNow };
        controller.SetIfNoneMatch(null);
        controller.SetIfModifiedSince(null);

        // Act
        bool result = controller.TestEntityIsModified(entity);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void OnActionExecuting_AddsModelError_WhenMultipleConditionalHeadersPresent()
    {
        // Arrange
        TestController controller = new();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["If-Match"] = "\"etag1\"";
        httpContext.Request.Headers["If-None-Match"] = "\"etag2\"";
        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(), new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary());
        var executingContext = new Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), controller);

        // Act
        controller.OnActionExecuting(executingContext);

        // Assert
        executingContext.ModelState.IsValid.Should().BeFalse();
        executingContext.ModelState[string.Empty]?.Errors.Should().Contain(e => e.ErrorMessage.Contains("Only one of the conditional request headers"));
    }

    [Fact]
    public void OnActionExecuting_SetsConditionalHeaderValues_ForEachHeader()
    {
        // Arrange
        TestController controller = new();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["If-Match"] = "\"etag1\"";
        httpContext.Request.Headers["If-None-Match"] = "\"etag2\"";
        httpContext.Request.Headers["If-Modified-Since"] = DateTimeOffset.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
        httpContext.Request.Headers["If-Unmodified-Since"] = DateTimeOffset.UtcNow.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
        httpContext.Request.Headers["X-Include-Deleted"] = "true";
        var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(), new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary());
        var executingContext = new Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), controller);

        // Act
        controller.OnActionExecuting(executingContext);

        // Assert
        controller.GetIfMatch().Should().Be("\"etag1\"");
        controller.GetIfNoneMatch().Should().Be("\"etag2\"");
        controller.GetIfModifiedSince().Should().NotBeNull();
        controller.GetIfUnmodifiedSince().Should().NotBeNull();
        controller.ExposeIncludeDeletedEntities.Should().BeTrue();
    }

    [Fact]
    public void OnActionExecuting_SetsBadRequestResult_WhenModelStateInvalid()
    {
        // Arrange
        TestController controller = new();
        var httpContext = new DefaultHttpContext();
        var modelState = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();
        modelState.AddModelError("Test", "Error");
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor(), modelState);
        var executingContext = new ActionExecutingContext(actionContext, [], new Dictionary<string, object?>(), controller);

        // Act
        controller.OnActionExecuting(executingContext);

        // Assert
        executingContext.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}
