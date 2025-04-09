namespace ProPulse.Core.Models;

/// <summary>
/// Represents an attachment in the ProPulse system, such as images or other media files associated with various entities.
/// </summary>
public class Attachment : BaseEntity
{
    /// <summary>
    /// Gets or sets the ID of the entity this attachment belongs to.
    /// This forms part of a polymorphic association with the OwnerType property.
    /// </summary>
    public string OwnerId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the type of entity this attachment belongs to (e.g., "Article", "ApplicationUser").
    /// This forms part of a polymorphic association with the OwnerId property.
    /// </summary>
    public string OwnerType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the MIME type of the attachment (e.g., image/png, application/pdf).
    /// </summary>
    public string ContentType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the name used to reference the attachment within the owning entity.
    /// This must be unique within the scope of an owner entity.
    /// </summary>
    public string LogicalName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the path or URL to the stored attachment in media storage.
    /// </summary>
    public string StorageLocation { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the size of the attachment in bytes.
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of articles using this attachment as their cover image.
    /// </summary>
    public virtual ICollection<Article> ArticlesUsingAsCover { get; init; } = [];
}
