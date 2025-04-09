namespace ProPulse.Core;

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
}
