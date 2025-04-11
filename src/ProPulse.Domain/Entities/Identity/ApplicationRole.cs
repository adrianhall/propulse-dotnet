using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;

namespace ProPulse.Domain.Entities.Identity;

/// <summary>
/// Represents a role in the application with extended properties beyond the standard ASP.NET Identity role
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    /// <summary>
    /// Description of the role's purpose and permissions
    /// </summary>
    public string? Description { get; set; }
}
