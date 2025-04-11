using ProPulse.Domain.Entities.Identity;
using System.Collections.ObjectModel;

namespace ProPulse.Domain.Entities;

/// <summary>
/// Represents a category for organizing articles
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// The name of the category
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// URL-friendly version of the category name
    /// </summary>
    public string Slug { get; set; } = null!;

    /// <summary>
    /// Description of the category
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Navigation property for articles in this category
    /// </summary>
    public virtual ICollection<Article> Articles { get; set; } = new Collection<Article>();
}
