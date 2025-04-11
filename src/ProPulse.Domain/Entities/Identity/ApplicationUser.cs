using System;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Identity;
using ProPulse.Domain.Interfaces;

namespace ProPulse.Domain.Entities.Identity;

/// <summary>
/// Represents a user in the application with extended properties beyond the standard ASP.NET Identity user
/// </summary>
public class ApplicationUser : IdentityUser<Guid>, ISoftDelete
{
    /// <summary>
    /// Display name shown to other users
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Biography or description for the user, primarily for authors
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Date and time when the user was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the user was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Date and time when the user last logged in
    /// </summary>
    public DateTimeOffset? LastLoginAt { get; set; }

    /// <summary>
    /// Flag indicating if the user is deleted (soft delete)
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Date and time when the user was soft deleted
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// Navigation property for articles created by this user
    /// </summary>
    public virtual ICollection<Article> Articles { get; set; } = [];

    /// <summary>
    /// Navigation property for bookmarks created by this user
    /// </summary>
    public virtual ICollection<Bookmark> Bookmarks { get; set; } = [];

    /// <summary>
    /// Navigation property for reading history entries created by this user
    /// </summary>
    public virtual ICollection<ReadingHistory> ReadingHistory { get; set; } = [];

    /// <summary>
    /// Navigation property for refresh tokens owned by this user
    /// </summary>
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
