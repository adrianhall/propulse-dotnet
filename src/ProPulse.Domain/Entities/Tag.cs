using ProPulse.Domain.Entities.Identity;
using System.Collections.ObjectModel;

namespace ProPulse.Domain.Entities;

/// <summary>
/// Represents a tag that can be applied to articles
/// </summary>
public class Tag : BaseEntity
{
    /// <summary>
    /// The name of the tag
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// URL-friendly version of the tag name
    /// </summary>
    public string Slug { get; set; } = null!;

    /// <summary>
    /// Navigation property for article-tag relationships
    /// </summary>
    public virtual ICollection<Article> Articles { get; set; } = [];
}
