namespace ProPulse.Core.Models;

/// <summary>
/// Represents a comment on an article in the ProPulse platform.
/// </summary>
public class Comment : BaseEntity
{
    /// <summary>
    /// Gets or sets the content of the comment.
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the ID of the article this comment belongs to.
    /// </summary>
    public string ArticleId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the ID of the parent comment if this is a reply.
    /// Null if this is a top-level comment.
    /// </summary>
    public string? ParentCommentId { get; set; }
    
    /// <summary>
    /// Gets or sets the article this comment belongs to.
    /// </summary>
    public virtual Article? Article { get; set; }
    
    /// <summary>
    /// Gets or sets the parent comment if this is a reply.
    /// </summary>
    public virtual Comment? ParentComment { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of replies to this comment.
    /// </summary>
    public virtual ICollection<Comment> Replies { get; init; } = [];
}
