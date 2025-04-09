using Microsoft.AspNetCore.Identity;

namespace ProPulse.Core.Models.Identity;

/// <summary>
/// Represents a user in the ProPulse application.
/// Extends the ASP.NET Core Identity IdentityUser class with additional properties.
/// </summary>
public class ApplicationUser : IdentityUser
{    
    /// <summary>
    /// Gets or sets the display name of the user.
    /// This is the name that is shown to other users in the application.
    /// </summary>
    public string? DisplayName { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the attachment used as the user's profile picture.
    /// </summary>
    public string? ProfilePictureAttachmentId { get; set; }
    
    /// <summary>
    /// Gets or sets the user's biographical information.
    /// This can include a short description, job title, or other personal information.
    /// </summary>
    public string? Bio { get; set; }
}
