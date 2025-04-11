namespace ProPulse.Domain.Entities;

/// <summary>
/// Represents a bookmark of an article by a user
/// </summary>
public class Bookmark : BaseEntity
{
    /// <summary>
    /// The ID of the article that is bookmarked
    /// </summary>
    public Guid ArticleId { get; set; }

    /// <summary>
    /// Navigation property for the article that is bookmarked
    /// </summary>
    public virtual Article? Article { get; set; }
}
