using ProPulse.Common;

namespace ProPulse.Core.Models;

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
    /// Gets or sets the ID of the attachment used as the article's cover image.
    /// </summary>
    public string? CoverImageAttachmentId { get; set; }
    
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
    
    /// <summary>
    /// Gets or sets the collection of article tags associated with this article.
    /// </summary>
    public virtual ICollection<ArticleTag> ArticleTags { get; init; } = [];
    
    /// <summary>
    /// Gets or sets the collection of comments on this article.
    /// </summary>
    public virtual ICollection<Comment> Comments { get; init; } = [];
    
    /// <summary>
    /// Gets or sets the cover image attachment.
    /// </summary>
    public virtual Attachment? CoverImageAttachment { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of ratings for this article.
    /// </summary>
    public virtual ICollection<Rating> Ratings { get; init; } = [];
    
    /// <summary>
    /// Gets or sets the collection of social media posts promoting this article.
    /// </summary>
    public virtual ICollection<SocialMediaPost> SocialMediaPosts { get; init; } = [];
}
