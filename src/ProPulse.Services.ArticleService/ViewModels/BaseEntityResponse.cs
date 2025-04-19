namespace ProPulse.Services.ArticleService.ViewModels;

/// <summary>
/// The response model for an entity.  This is the base class for all responses
/// and contains copies of the entity's properties.  This is used to provide a consistent
/// response format across the API and to ensure that the response is not directly
/// exposed to the client.
/// </summary>
public abstract class BaseEntityResponse
{
    /// <summary>
    /// A unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The date and time when the entity was created.  This is stored
    /// in UTC format with microsecond precision.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// The date and time when the entity was deleted.  This is stored
    /// in UTC format with microsecond precision, if set.  If not set,
    /// then the entity is not deleted.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// Indicates whether the entity is soft-deleted. If true, the entity is considered deleted
    /// and will be filtered from queries by default.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// The date and time when the entity was last updated. This is stored
    /// in UTC format with microsecond precision.  This is automatically
    /// maintained by the database and is used to provide the Last-Modified
    /// header when the entity is returned in a response.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// The row version for the entity. This is automatically maintained
    /// by the database to ensure concurrency control and is used to provide
    /// the ETag header when the entity is returned in a response.
    /// </summary>
    public byte[] Version { get; set; } = [];
}