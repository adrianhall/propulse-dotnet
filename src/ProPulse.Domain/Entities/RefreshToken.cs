using ProPulse.Domain.Entities.Identity;

namespace ProPulse.Domain.Entities;

/// <summary>
/// Represents a refresh token for JWT authentication
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Unique identifier for the refresh token
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the user who owns this refresh token
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// The token value used for refreshing authentication
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// Date and time when the token expires
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Date and time when the token was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// IP address that created the token
    /// </summary>
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// Date and time when the token was revoked
    /// </summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>
    /// IP address that revoked the token
    /// </summary>
    public string? RevokedByIp { get; set; }

    /// <summary>
    /// Token that replaced this one when refreshed
    /// </summary>
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Reason the token was revoked
    /// </summary>
    public string? ReasonRevoked { get; set; }

    /// <summary>
    /// Navigation property for the user who owns this refresh token
    /// </summary>
    public virtual ApplicationUser? User { get; set; }

    /// <summary>
    /// Determines if the token is active (not expired and not revoked)
    /// </summary>
    public bool IsActive => RevokedAt == null && !IsExpired;

    /// <summary>
    /// Determines if the token is expired
    /// </summary>
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
}
