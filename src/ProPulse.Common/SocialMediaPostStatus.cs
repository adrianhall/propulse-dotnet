namespace ProPulse.Common;

/// <summary>
/// Represents the possible states of a social media post in the workflow.
/// </summary>
public enum SocialMediaPostStatus
{
    /// <summary>
    /// The post is pending approval before it can be published.
    /// </summary>
    Pending,
    
    /// <summary>
    /// The post has been approved and is ready to be published.
    /// </summary>
    Approved,
    
    /// <summary>
    /// The post has been rejected and will not be published.
    /// </summary>
    Rejected,
    
    /// <summary>
    /// The post has been published to the social media platform.
    /// </summary>
    Published
}
