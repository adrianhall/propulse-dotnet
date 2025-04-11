namespace ProPulse.Domain.Entities;

/// <summary>
/// Represents the possible types of attachments for articles
/// </summary>
public enum AttachmentType
{
    /// <summary>
    /// Primary image displayed in article listings and headers
    /// </summary>
    FeaturedImage,

    /// <summary>
    /// Image embedded within article content
    /// </summary>
    ContentImage
}
