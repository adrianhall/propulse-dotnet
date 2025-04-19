namespace ProPulse.Services.ArticleService.ViewModels;

using System;

/// <summary>
/// Represents the response returned for an article.
/// </summary>
public sealed class ArticleResponse : BaseEntityResponse
{
    /// <summary>
    /// Gets or sets the title of the article.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of the article in Markdown.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the article was published.
    /// </summary>
    public DateTimeOffset? PublishedAt { get; set; }
}
