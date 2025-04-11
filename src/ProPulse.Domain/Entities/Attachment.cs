namespace ProPulse.Domain.Entities;

/// <summary>
/// Represents a media attachment associated with an article
/// </summary>
public class Attachment : BaseEntity
{
    /// <summary>
    /// The ID of the article this attachment belongs to
    /// </summary>
    public Guid ArticleId { get; set; }

    /// <summary>
    /// The type of attachment (e.g., FeaturedImage, ContentImage)
    /// </summary>
    public AttachmentType? AttachmentType { get; set; }

    /// <summary>
    /// User-friendly name for referencing the attachment
    /// </summary>
    public string SymbolicName { get; set; } = null!;

    /// <summary>
    /// Internal storage path for the attachment
    /// </summary>
    public string StorageLocation { get; set; } = null!;

    /// <summary>
    /// MIME type of the attachment
    /// </summary>
    public string ContentType { get; set; } = null!;

    /// <summary>
    /// Size of the attachment in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Navigation property for the article this attachment belongs to
    /// </summary>
    public virtual Article? Article { get; set; }
}
