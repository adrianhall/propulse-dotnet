using ProPulse.Domain.Entities.Identity;
using System.Collections.ObjectModel;

namespace ProPulse.Domain.Entities;

/// <summary>
/// Represents an article in the content management system
/// </summary>
public class Article : BaseEntity
{
    /// <summary>
    /// The title of the article
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// URL-friendly version of the article title
    /// </summary>
    public string Slug { get; set; } = null!;

    /// <summary>
    /// The main content of the article in Markdown format
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// A short summary or excerpt of the article
    /// </summary>
    public string? Excerpt { get; set; }

    /// <summary>
    /// The number of times the article has been viewed
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// The ID of the category this article belongs to
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// The publication status of the article
    /// </summary>
    public ArticleStatus Status { get; set; }

    /// <summary>
    /// The date and time when the article was published
    /// </summary>
    public DateTimeOffset? PublishedAt { get; set; }

    /// <summary>
    /// Navigation property for the category this article belongs to
    /// </summary>
    public virtual Category? Category { get; set; }

    /// <summary>
    /// Navigation property for tags applied to this article
    /// </summary>
    public virtual ICollection<Tag> Tags { get; set; } = [];

    /// <summary>
    /// Navigation property for attachments associated with this article
    /// </summary>
    public virtual ICollection<Attachment> Attachments { get; set; } = [];

    /// <summary>
    /// Navigation property for bookmarks of this article
    /// </summary>
    public virtual ICollection<Bookmark> Bookmarks { get; set; } = [];

    /// <summary>
    /// Navigation property for reading history entries for this article
    /// </summary>
    public virtual ICollection<ReadingHistory> ReadingHistories { get; set; } = [];
}
