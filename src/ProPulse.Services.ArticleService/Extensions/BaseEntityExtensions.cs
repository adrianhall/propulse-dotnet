using ProPulse.Core.Entities;
using ProPulse.Services.ArticleService.ViewModels;

namespace ProPulse.Services.ArticleService.Extensions;

/// <summary>
/// A set of helper extensions for the <see cref="BaseEntity"/> class.
/// </summary>
public static class BaseEntityExtensions
{
    /// <summary>
    /// Gets the string representation of the entity Version as an ETag
    /// value.
    /// </summary>
    /// <param name="entity">The entity to convert.</param>
    /// <returns>The ETag value.</returns>
    public static string GetETag(this BaseEntity entity)
        => $"\"{Convert.ToBase64String(entity.Version)}\"";

    /// <summary>
    /// Gets the string representation of the entity Version as an ETag
    /// value.
    /// </summary>
    /// <param name="entity">The entity to convert.</param>
    /// <returns>The ETag value.</returns>
    public static string GetETag(this BaseEntityResponse entity)
        => $"\"{Convert.ToBase64String(entity.Version)}\"";
}