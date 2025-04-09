using ProPulse.Common;

namespace ProPulse.Core;

/// <summary>
/// Represents an article in the ProPulse content management system.
/// </summary>
public class Article : BaseEntity
{
    /// <summary>
    /// Gets or sets the title of the article.
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the URL-friendly slug for the article.
    /// </summary>
    public string Slug { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the summary of the article.
    /// </summary>
    public string Summary { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the main content of the article.
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the URL to the article's cover image.
    /// </summary>
    public string? CoverImageUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the current status of the article.
    /// </summary>
    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;
    
    /// <summary>
    /// Gets or sets the date and time when the article was or will be published.
    /// </summary>
    public DateTimeOffset? PublishedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time until which the article will remain published.
    /// If null, the article has no expiration date.
    /// </summary>
    public DateTimeOffset? PublishedUntil { get; set; }
}
