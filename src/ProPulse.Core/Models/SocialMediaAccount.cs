using ProPulse.Common;

namespace ProPulse.Core.Models;

/// <summary>
/// Represents a social media account that can be used to publish content.
/// </summary>
public class SocialMediaAccount : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the social media account.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the platform this account belongs to.
    /// </summary>
    public SocialMediaPlatform Platform { get; set; }
    
    /// <summary>
    /// Gets or sets the unique identifier or handle for this account on the platform.
    /// </summary>
    public string Handle { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a brief description of this social media account.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the attachment used as the account's profile picture.
    /// </summary>
    public string? ProfilePictureAttachmentId { get; set; }
    
    /// <summary>
    /// Gets or sets the authentication token for API access.
    /// Note: In a production environment, this should be encrypted or stored in a secure vault.
    /// </summary>
    public string? AuthToken { get; set; }
    
    /// <summary>
    /// Gets or sets whether this account is currently active and available for use.
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the profile picture attachment for this social media account.
    /// </summary>
    public virtual Attachment? ProfilePictureAttachment { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of social media posts for this account.
    /// </summary>
    public virtual ICollection<SocialMediaPost> Posts { get; set; } = [];
}
