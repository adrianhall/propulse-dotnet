using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProPulse.Core;
using ProPulse.Core.Entities;
using ProPulse.Services.ArticleService.Extensions;
using ProPulse.Services.ArticleService.ViewModels;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ProPulse.Services.ArticleService.Controllers;

/// <summary>
/// A base class for API controllers in the ProPulse service, providing common functionality,
/// such as RFC 9110 compatible conditional request processing.
/// </summary>
public abstract partial class ApiServiceController : Controller
{
    /// <summary>
    /// The name of the header used to indicate whether deleted entities should be included in the response.
    /// </summary>
    private const string IncludeDeletedHeaderName = "X-Include-Deleted";

    /// <summary>
    /// The name of the header used to indicate the ETag value for conditional requests.
    /// </summary>
    private const string IfMatchHeaderName = "If-Match";

    /// <summary>
    /// The name of the header used to indicate the ETag value for conditional requests.
    /// </summary>
    private const string IfNoneMatchHeaderName = "If-None-Match";

    /// <summary>
    /// The name of the header used to indicate the last modified date for conditional requests.
    /// </summary>
    private const string IfModifiedSinceHeaderName = "If-Modified-Since";

    /// <summary>
    /// The name of the header used to indicate the last modified date for conditional requests.
    /// </summary>
    private const string IfUnmodifiedSinceHeaderName = "If-Unmodified-Since";


    /// <summary>
    /// True if the request is an RFC 9110 conditional request.
    /// </summary>
    protected bool IsConditionalRequest { get; private set; } = false;

    /// <summary>
    /// If true, deleted entities are included in the query results.
    /// </summary>
    protected bool IncludeDeletedEntities { get; private set; } = false;

    /// <summary>
    /// The headers for the conditional request.
    /// </summary>
    protected ConditionalRequestHeaders ConditionalHeaderValues { get; private set; } = new();

    /// <summary>
    /// Called when the action is executing.
    /// </summary>
    /// <remarks>
    /// This method is called before the action method is executed, and parses the request headers
    /// to determine if the request is a conditional request.
    /// <param name="context"></p>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        context.ValidateBooleanHeader(IncludeDeletedHeaderName, v => IncludeDeletedEntities = v);
        context.ValidateETagHeader(IfMatchHeaderName, v => ConditionalHeaderValues.IfMatch = v);
        context.ValidateETagHeader(IfNoneMatchHeaderName, v => ConditionalHeaderValues.IfNoneMatch = v);
        context.ValidateDateHeader(IfModifiedSinceHeaderName, v => ConditionalHeaderValues.IfModifiedSince = v);
        context.ValidateDateHeader(IfUnmodifiedSinceHeaderName, v => ConditionalHeaderValues.IfUnmodifiedSince = v);

        IsConditionalRequest = ConditionalHeaderValues.IsConditionalRequest;
        if (ConditionalHeaderValues.HasMultipleHeaders)
        {
            context.ModelState.AddModelError("", "Only one of the conditional request headers can be specified.");
        }

        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(ModelState);
            return; // Short-circuit the action execution if the model state is invalid.
        }

        base.OnActionExecuting(context);
    }

    /// <summary>
    /// Called after the action executes. Adds conditional headers if applicable.
    /// </summary>
    /// <param name="context">The action executed context.</param>
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult result && result.Value is BaseEntityResponse entity)
        {
            if (result.StatusCode is StatusCodes.Status200OK or StatusCodes.Status201Created)
            {
                IHeaderDictionary headers = context.HttpContext.Response.Headers;

                headers.ETag = entity.GetETag();
                headers.LastModified = entity.UpdatedAt.ToUniversalTime().ToString("R", CultureInfo.InvariantCulture);
                headers["Last-Modified-Timestamp"] = entity.UpdatedAt.ToUniversalTime().ToString(AppConstants.TimestampFormat, CultureInfo.InvariantCulture);
            }
        }
        base.OnActionExecuted(context);
    }

    /// <summary>
    /// Determines if the entity is modified according to the conditional request headers.
    /// </summary>
    /// <param name="entity">The entity being processed.</param>
    /// <returns>true if the entity is modified; false otherwise.</returns>
    protected bool EntityIsModified(BaseEntity entity)
    {
        // The ETag match is always checked first.
        if (ConditionalHeaderValues.IfNoneMatch is not null)
        {
            return ConditionalHeaderValues.IfNoneMatch != "*"
                && ConditionalHeaderValues.IfNoneMatch != entity.GetETag();
        }

        if (ConditionalHeaderValues.IfModifiedSince is not null)
        {
            return ConditionalHeaderValues.IfModifiedSince <= entity.UpdatedAt;
        }

        return false;
    }

    /// <summary>
    /// The conditional request headers that are parsed from the request.
    /// </summary>
    protected sealed class ConditionalRequestHeaders
    {
        /// <summary>
        /// The If-Match header value.
        /// </summary>
        public string? IfMatch { get; set; }

        /// <summary>
        /// The If-None-Match header value.
        /// </summary>
        public string? IfNoneMatch { get; set; }

        /// <summary>
        /// The If-Modified-Since or If-Modified-Since-Timestamp header value.
        /// </summary>
        public DateTimeOffset? IfModifiedSince { get; set; }

        /// <summary>
        /// The If-Unmodified-Since or If-Unmodified-Since-Timestamp header value.
        /// </summary>
        public DateTimeOffset? IfUnmodifiedSince { get; set; }

        /// <summary>
        /// True if the request has multiple conditional request headers.
        /// </summary>
        public bool HasMultipleHeaders
            => Count > 1;

        /// <summary>
        /// True if the request is a conditional request.
        /// </summary>
        public bool IsConditionalRequest
            => IfMatch is not null
            || IfNoneMatch is not null
            || IfModifiedSince is not null
            || IfUnmodifiedSince is not null;

        /// <summary>
        /// The number of conditional request headers that are present in the request.
        /// </summary>
        public int Count
            => (IfMatch is not null ? 1 : 0) + (IfNoneMatch is not null ? 1 : 0)
            + (IfModifiedSince is not null ? 1 : 0) + (IfUnmodifiedSince is not null ? 1 : 0);
    }

    [GeneratedRegex(@"^(\*|""[0-9a-zA-Z]+"")$")]
    private static partial Regex ETagRegex();
}