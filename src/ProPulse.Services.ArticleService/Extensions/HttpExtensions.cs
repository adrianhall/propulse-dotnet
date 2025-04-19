using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProPulse.Core;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ProPulse.Services.ArticleService.Extensions;

/// <summary>
/// A set of helper extensions for working with HTTP messages.
/// </summary>
public static partial class HttpExtensions
{
    private static readonly Regex ETagRegex = new(@"^(\*|""[0-9a-zA-Z]+=*"")$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    /// <summary>
    /// Returns the single value of a header in the request if it exists.  If there are multiple
    /// headers, then an error is added to the ModelState.  If no header exists, then null is
    /// returned.
    /// </summary>
    /// <param name="context">The context holding the request.</param>
    /// <param name="headerName">The name of the header.</param>
    /// <returns>The value of the header, or null if it does not exist.</returns>
    public static string? GetRequestHeader(this ActionExecutingContext context, string headerName)
        => GetRequestHeader(context.HttpContext, headerName, context.ModelState);

    /// <summary>
    /// Returns the single value of a header in the request if it exists.  If there are multiple
    /// headers, then an error is added to the ModelState.  If no header exists, then null is
    /// returned.
    /// </summary>
    /// <param name="context">The context holding the request.</param>
    /// <param name="headerName">The name of the header.</param>
    /// <param name="modelState">The model state to add errors to.</param>
    /// <returns>The value of the header, or null if it does not exist.</returns>
    public static string? GetRequestHeader(this HttpContext context, string headerName, ModelStateDictionary modelState)
        => GetRequestHeader(context.Request, headerName, modelState);

    /// <summary>
    /// Returns the single value of a header in the request if it exists.  If there are multiple
    /// headers, then an error is added to the ModelState.  If no header exists, then null is
    /// returned.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequest"> holding the request.</param>
    /// <param name="headerName">The name of the header.</param>
    /// <param name="modelState">The model state to add errors to.</param>
    /// <returns>The value of the header, or null if it does not exist.</returns>
    public static string? GetRequestHeader(this HttpRequest request, string headerName, ModelStateDictionary modelState)
    {
        if (request.Headers.TryGetValue(headerName, out var values))
        {
            if (values.Count > 1)
            {
                modelState.AddModelError(headerName, $"Header '{headerName}' has multiple values. Only one value is allowed.");
            }

            return values.FirstOrDefault();
        }
        return null;
    }

    /// <summary>
    /// Validates a boolean header on the request.  If the header is present and parses as a boolean,
    /// the action is invoked with the parsed value.  If the header is not present, or if it cannot be parsed,
    /// an error is added to the ModelState. 
    /// </summary>
    /// <param name="context">The executing context</param>
    /// <param name="headerName">The name of the header</param>
    /// <param name="action">The action to invoke for a valid header</param>
    public static void ValidateBooleanHeader(this ActionExecutingContext context, string headerName, Action<bool> action)
    {
        string? headerValue = context.GetRequestHeader(headerName);
        if (headerValue is not null)
        {
            if (bool.TryParse(headerValue, out bool parsedValue))
            {
                action.Invoke(parsedValue);
            }
            else
            {
                context.ModelState.AddModelError(headerName, $"Expected 'true' or 'false'.");
            }
        }
    }

    /// <summary>
    /// Validates an ETag header on the request.  If the header is present and parses as an ETag,
    /// the action is invoked with the parsed value.  If the header is not present, or if it cannot be parsed,
    /// an error is added to the ModelState. 
    /// </summary>
    /// <param name="context">The executing context</param>
    /// <param name="headerName">The name of the header</param>
    /// <param name="action">The action to invoke for a valid header</param>
    public static void ValidateETagHeader(this ActionExecutingContext context, string headerName, Action<string> action)
    {
        string? headerValue = context.GetRequestHeader(headerName);
        if (headerValue is not null)
        {

            if (ETagRegex.IsMatch(headerValue))
            {
                action.Invoke(headerValue);
            }
            else
            {
                context.ModelState.AddModelError(headerName, $"Invalid ETag format. Expected '*' or a quoted base-64 string.");
            }
        }
    }

    /// <summary>
    /// Validates a pair of date headers on the request.  If the header is present and parses correctly,
    /// the action is invoked with the parsed value.  If the header is not present, or if it cannot be parsed,
    /// an error is added to the ModelState. Note that only one of the two headers can be specified.
    /// </summary>
    /// <param name="context">The executing context</param>
    /// <param name="headerName">The name of the header</param>
    /// <param name="action">The action to invoke for a valid header</param>
    public static void ValidateDateHeader(this ActionExecutingContext context, string headerName, Action<DateTimeOffset> action)
    {
        string timestampHeaderName = headerName + "-Timestamp";

        string? headerValue = context.GetRequestHeader(headerName);
        string? timestampHeaderValue = context.GetRequestHeader(timestampHeaderName);
        if (headerValue is not null && timestampHeaderValue is not null)
        {
            context.ModelState.AddModelError(headerName, $"Only one of '{headerName}' or '{timestampHeaderName}' can be specified.");
        }
        else if (headerValue is not null)
        {
            if (DateTimeOffset.TryParseExact(headerValue, "R", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset rfcValue))
            {
                action.Invoke(rfcValue);
            }
            else
            {
                context.ModelState.AddModelError(headerName, $"Invalid date format. Expected RFC 1123 format.");
            }
        }
        else if (timestampHeaderValue is not null)
        {
            if (DateTimeOffset.TryParseExact(timestampHeaderValue, AppConstants.TimestampFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset timestampValue))
            {
                action.Invoke(timestampValue);
            }
            else
            {
                context.ModelState.AddModelError(timestampHeaderName, $"Invalid date format. Expected a valid date string.");
            }
        }
    }
}