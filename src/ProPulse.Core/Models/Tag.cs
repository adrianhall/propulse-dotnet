namespace ProPulse.Core.Models;

/// <summary>
/// Represents a tag that can be applied to articles for categorization.
/// </summary>
public class Tag : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the tag.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the slug version of the tag name for URLs.
    /// </summary>
    public string Slug { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets an optional description of the tag.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of article tags associated with this tag.
    /// </summary>
    public virtual ICollection<ArticleTag> ArticleTags { get; init; } = [];
}
