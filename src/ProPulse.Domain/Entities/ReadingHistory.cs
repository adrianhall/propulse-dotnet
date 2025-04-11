namespace ProPulse.Domain.Entities;

/// <summary>
/// Represents an entry in a user's reading history
/// </summary>
public class ReadingHistory : BaseEntity
{
    /// <summary>
    /// The ID of the article that was read
    /// </summary>
    public Guid ArticleId { get; set; }

    /// <summary>
    /// Navigation property for the article that was read
    /// </summary>
    public virtual Article? Article { get; set; }
}
