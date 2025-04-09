namespace ProPulse.Common;

/// <summary>
/// Represents the possible states of an article in the publishing workflow.
/// </summary>
public enum ArticleStatus
{
    /// <summary>
    /// Article is in draft state and not publicly available.
    /// </summary>
    Draft,
    
    /// <summary>
    /// Article is published and publicly available.
    /// </summary>
    Published,
    
    /// <summary>
    /// Article is scheduled for future publication.
    /// </summary>
    Scheduled
}
