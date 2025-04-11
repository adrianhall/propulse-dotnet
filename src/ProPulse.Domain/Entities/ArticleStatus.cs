namespace ProPulse.Domain.Entities;

/// <summary>
/// Represents the possible publication statuses of an article
/// </summary>
public enum ArticleStatus
{
    /// <summary>
    /// Article is in draft state and not publicly visible
    /// </summary>
    Draft,

    /// <summary>
    /// Article is published and publicly visible
    /// </summary>
    Published,

    /// <summary>
    /// Article is archived and may have limited visibility
    /// </summary>
    Archived
}
