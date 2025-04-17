namespace ProPulse.Core.Entities;

/// <summary>
/// Represents an article within the database.
/// </summary>
public class Article : BaseEntity
{
    /// <summary>
    /// The title of the article.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The content of the article in Markdown.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// The date and time when the article was published.
    /// </summary>
    public DateTimeOffset? PublishedAt { get; set; }
}