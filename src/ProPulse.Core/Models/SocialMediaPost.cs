using ProPulse.Common;

namespace ProPulse.Core.Models;

/// <summary>
/// Represents a post to be published on a social media platform.
/// </summary>
public class SocialMediaPost : BaseEntity
{
    /// <summary>
    /// Gets or sets the content of the social media post.
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the ID of the social media account this post is associated with.
    /// </summary>
    public string SocialMediaAccountId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the optional ID of the article this post is promoting.
    /// </summary>
    public string? ArticleId { get; set; }
    
    /// <summary>
    /// Gets or sets the status of this social media post.
    /// </summary>
    public SocialMediaPostStatus Status { get; set; } = SocialMediaPostStatus.Pending;
    
    /// <summary>
    /// Gets or sets the scheduled date and time for publishing this post.
    /// </summary>
    public DateTimeOffset? ScheduledPublishDate { get; set; }
    
    /// <summary>
    /// Gets or sets the actual date and time when this post was published.
    /// </summary>
    public DateTimeOffset? PublishedDate { get; set; }
    
    /// <summary>
    /// Gets or sets the external ID assigned by the social media platform after publishing.
    /// </summary>
    public string? ExternalPostId { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the attachment to include with this post.
    /// </summary>
    public string? ImageAttachmentId { get; set; }
    
    /// <summary>
    /// Gets or sets any additional metadata for this post in JSON format.
    /// </summary>
    public string? Metadata { get; set; }
    
    /// <summary>
    /// Gets or sets the social media account this post belongs to.
    /// </summary>
    public virtual SocialMediaAccount? SocialMediaAccount { get; set; }
    
    /// <summary>
    /// Gets or sets the article this post is promoting.
    /// </summary>
    public virtual Article? Article { get; set; }
    
    /// <summary>
    /// Gets or sets the image attachment for this post.
    /// </summary>
    public virtual Attachment? ImageAttachment { get; set; }
}
