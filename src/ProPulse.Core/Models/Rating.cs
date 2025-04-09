namespace ProPulse.Core.Models;

/// <summary>
/// Represents a user rating for an article in the ProPulse platform.
/// </summary>
public class Rating : BaseEntity
{
    /// <summary>
    /// Gets or sets the ID of the article this rating belongs to.
    /// </summary>
    public string ArticleId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the score given to the article by the user.
    /// </summary>
    public int Score { get; set; }
    
    /// <summary>
    /// Gets or sets the article this rating belongs to.
    /// </summary>
    public virtual Article? Article { get; set; }
}
