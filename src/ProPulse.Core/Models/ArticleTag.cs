namespace ProPulse.Core.Models;

/// <summary>
/// Represents the many-to-many relationship between articles and tags.
/// </summary>
public class ArticleTag : BaseEntity
{
    /// <summary>
    /// Gets or sets the ID of the article.
    /// </summary>
    public string ArticleId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the ID of the tag.
    /// </summary>
    public string TagId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the related article.
    /// </summary>
    public virtual Article? Article { get; set; }
    
    /// <summary>
    /// Gets or sets the related tag.
    /// </summary>
    public virtual Tag? Tag { get; set; }
}
