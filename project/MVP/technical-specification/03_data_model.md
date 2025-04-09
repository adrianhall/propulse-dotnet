# 3. Data Model

## 3.1. Tables

### BaseEntity

This is the base class that all entities derive from, providing common auditing fields.

| Column | .NET Type | PostgreSQL Type | Constraints | Description |
|--------|-----------|----------------|-------------|-------------|
| Id | string | text | PK | Unique identifier for the entity |
| CreatedAt | DateTimeOffset | timestamptz | NOT NULL | Entity creation timestamp |
| UpdatedAt | DateTimeOffset | timestamptz | NOT NULL | Entity last update timestamp |
| Version | byte[] | bytea | NOT NULL | Concurrency token for optimistic locking |
| CreatedById | string | text | FK, NULL | Reference to User.Id who created the entity |
| UpdatedById | string | text | FK, NULL | Reference to User.Id who last updated the entity |

* The `CreatedAt` and `UpdatedAt` columns are maintained by the database server using a trigger.
* The `Version` is a row version, normally maintained by the database server.

### AspNetUsers

The identity user entity, integrating with ASP.NET Identity `IdentityUser` class.

Columns from the `IdentityUser` class:

| Column | .NET Type | PostgreSQL Type | Constraints | Description |
|--------|-----------|----------------|-------------|-------------|
| Id | string | text | PK | Unique identifier for the user |
| UserName | string | citext | NOT NULL, UNIQUE | User's login name (case insensitive) |
| Email | string | citext | NOT NULL, UNIQUE | User's email address (case insensitive) |
| EmailConfirmed | bool | boolean | NOT NULL | Indicates if email is confirmed |
| PasswordHash | string | text | NULL | Hashed password |
| SecurityStamp | string | text | NULL | Security stamp for authentication |
| NormalizedUserName | string | text | NOT NULL | Normalized username for searching |
| NormalizedEmail | string | text | NOT NULL | Normalized email for searching |
| ConcurrencyStamp | string | text | NULL | Concurrency stamp |
| LockoutEnd | DateTimeOffset | timestamptz | NULL | When lockout ends |
| LockoutEnabled | bool | boolean | NOT NULL | Whether account can be locked out |
| AccessFailedCount | int | integer | NOT NULL | Count of failed access attempts |

Columns from the ProPulse customizations:

| Column | .NET Type | PostgreSQL Type | Constraints | Description |
|--------|-----------|----------------|-------------|-------------|
| DisplayName | string | text | NOT NULL | Name shown on articles and comments |
| ProfilePictureUrl | string | citext | NULL | URL to user's profile picture |
| Bio | string | text | NULL | Short user biography |

### Other Identity tables

The `AspNetRoles` table matches the `IdentityRole` class from ASP.NET Identity. Other 
ASP.NET Identity required tables will be created as per the ASP.NET Identity system 
requirements.

### Article (extends BaseEntity)

| Column | .NET Type | PostgreSQL Type | Constraints | Description |
|--------|-----------|----------------|-------------|-------------|
| Id | string | text | PK | Unique identifier for the article |
| Title | string | text | NOT NULL | Article title |
| Slug | string | citext | NOT NULL, UNIQUE | URL-friendly title for SEO (case insensitive) |
| Summary | string | text | NOT NULL | Brief summary of article content |
| Content | string | text | NOT NULL | Full article content (Markdown) |
| CoverImageUrl | string | citext | NULL | URL to article cover image |
| Status | ArticleStatus | article_status | NOT NULL | Draft, Published, Scheduled |
| PublishedAt | DateTimeOffset | timestamptz | NULL | When article was/will be published |
| PublishedUntil | DateTimeOffset | timestamptz | NULL | When article will expire if temporary |

* The `PublishedAt` and `PublishedUntil` columns will be maintained according to rules related to changes in the `Status` column and whether the value has been provided or not. This will be done via a trigger by the database server.

### Comment (extends BaseEntity)

| Column | .NET Type | PostgreSQL Type | Constraints | Description |
|--------|-----------|----------------|-------------|-------------|
| Id | string | text | PK | Unique identifier for the comment |
| Content | string | text | NOT NULL | Markdown-flavor with no images or links |
| ArticleId | string | text | FK, NOT NULL | Reference to Article.Id |
| ParentCommentId | string | text | FK, NULL | For nested comments |

### Rating (extends BaseEntity)

| Column | .NET Type | PostgreSQL Type | Constraints | Description |
|--------|-----------|----------------|-------------|-------------|
| Id | string | text | PK | Unique identifier for the rating |
| ArticleId | string | text | FK, NOT NULL | Reference to Article.Id |
| Score | int | integer | NOT NULL, CHECK (Score BETWEEN 1 AND 5) | Rating value (1-5) |

### SocialMediaAccount (extends BaseEntity)

| Column | .NET Type | PostgreSQL Type | Constraints | Description |
|--------|-----------|----------------|-------------|-------------|
| Id | string | text | PK | Unique identifier for the account |
| Name | string | text | NOT NULL | Friendly name for the account |
| Platform | SocialMediaPlatform | social_media_platform | NOT NULL | Facebook, Twitter, LinkedIn, etc. |
| AccessToken | string | text | NOT NULL | Encrypted access token |
| RefreshToken | string | text | NULL | Encrypted refresh token |
| TokenExpiry | DateTimeOffset | timestamptz | NULL | When access token expires |

### SocialMediaPost (extends BaseEntity)

| Column | .NET Type | PostgreSQL Type | Constraints | Description |
|--------|-----------|----------------|-------------|-------------|
| Id | string | text | PK | Unique identifier for the post |
| Content | string | text | NOT NULL | Post text content |
| ArticleId | string | text | FK, NOT NULL | Reference to Article.Id |
| SocialMediaAccountId | string | text | FK, NOT NULL | Reference to SocialMediaAccount.Id |
| ScheduledDate | DateTimeOffset | timestamptz | NOT NULL | When to publish the post |
| Status | SocialMediaPostStatus | social_media_post_status | NOT NULL | Pending, Approved, Rejected, Published |
| ApprovedBy | string | text | FK, NULL | Reference to AspNetUsers.Id |
| ExternalPostId | string | text | NULL | ID from social platform after publishing |
| PlatformMetadata | Dictionary<string, object> | jsonb | NULL | Platform-specific metadata |
| AnalyticsData | Dictionary<string, object> | jsonb | NULL | Analytics data from the social platform |

### Attachment (extends BaseEntity)

| Column | .NET Type | PostgreSQL Type | Constraints | Description |
|--------|-----------|----------------|-------------|-------------|
| Id | string | text | PK | Unique identifier for the attachment |
| OwnerId | string | text | NOT NULL | ID of the entity that owns this attachment |
| OwnerType | string | text | NOT NULL | Type of entity that owns this attachment (e.g., "Article", "ApplicationUser") |
| ContentType | string | text | NOT NULL | MIME type of the attachment (e.g., image/png) |
| LogicalName | string | text | NOT NULL | Name used to reference the attachment within the owning entity |
| StorageLocation | string | text | NOT NULL | Path or URL to the stored attachment in media storage |
| FileSize | long | bigint | NOT NULL | Size of the attachment in bytes |

* The `LogicalName` must be unique within the scope of an owner entity. This is enforced by a unique constraint on (OwnerType, OwnerId, LogicalName).
* The `OwnerType` and `OwnerId` form a polymorphic association that allows attachments to be associated with different entity types (e.g., Article, ApplicationUser, SocialMediaAccount, SocialMediaPost).

## 3.2. Entity Relationship Diagram

```mermaid
erDiagram
    BaseEntity {
        string Id PK
        DateTimeOffset CreatedAt
        DateTimeOffset UpdatedAt
        byte[] Version
        string CreatedById FK
        string UpdatedById FK
    }
    
    AspNetUsers ||--o{ Article : "creates"
    AspNetUsers ||--o{ Comment : "creates"
    AspNetUsers ||--o{ Rating : "gives"
    AspNetUsers ||--o{ SocialMediaPost : "approves"
    AspNetUsers }|..|{ AspNetRoles : "has"    Article ||--o{ Comment : "has"
    Article ||--o{ Rating : "receives"
    Article ||--o{ SocialMediaPost : "promoted by"
    Article ||--o{ Attachment : "has"
    Comment ||--o{ Comment : "has replies"
    SocialMediaAccount ||--o{ SocialMediaPost : "used for"
    AspNetUsers ||--o{ Attachment : "has profile picture"
    SocialMediaAccount ||--o{ Attachment : "has profile picture"
    SocialMediaPost ||--o{ Attachment : "has image"
    BaseEntity ||--o{ Article : "extends"
    BaseEntity ||--o{ Comment : "extends"
    BaseEntity ||--o{ Rating : "extends"
    BaseEntity ||--o{ SocialMediaAccount : "extends"
    BaseEntity ||--o{ SocialMediaPost : "extends"
    BaseEntity ||--o{ Attachment : "extends"
    
    AspNetUsers {
        string Id PK
        string UserName CITEXT
        string Email CITEXT
        string DisplayName
        string ProfilePictureUrl CITEXT
        string Bio
        bool EmailConfirmed
    }
    
    AspNetRoles {
        string Id PK
        string Name CITEXT
        string NormalizedName
    }
    
    Article {
        string Id PK
        string Title
        string Slug CITEXT
        string Summary
        string Content
        string CoverImageUrl CITEXT
        enum Status
        DateTimeOffset PublishedAt
        DateTimeOffset "PublishedUntil"
    }
    
    Comment {
        string Id PK
        string Content
        string ArticleId FK
        string ParentCommentId FK
    }
    
    Rating {
        string Id PK
        string ArticleId FK
        int Score
    }
    
    SocialMediaAccount {
        string Id PK
        string Name
        enum Platform
        string AccessToken
        string RefreshToken
        DateTimeOffset TokenExpiry
    }
      SocialMediaPost {
        string Id PK
        string Content
        string ArticleId FK
        string SocialMediaAccountId FK
        DateTimeOffset ScheduledDate
        enum Status
        string ApprovedBy FK
        string ExternalPostId
        jsonb PlatformMetadata
        jsonb AnalyticsData
    }
    
    Attachment {
        string Id PK
        string ArticleId FK
        string ContentType
        string LogicalName
        string StorageLocation
        long FileSize
    }
```

## 3.3. PostgreSQL Implementation Notes

### Enum Types

The system uses several PostgreSQL enum types to enforce constraints at the database level:

```sql
CREATE TYPE article_status AS ENUM ('Draft', 'Published', 'Scheduled');
CREATE TYPE social_media_platform AS ENUM ('Facebook', 'Twitter', 'LinkedIn', 'Instagram', 'Mastodon');
CREATE TYPE social_media_post_status AS ENUM ('Pending', 'Approved', 'Rejected', 'Published');
```

Using enum types provides several benefits:
- Type safety at the database level
- Storage efficiency (stored as integers internally)
- Self-documenting constraints

### Triggers

The following triggers are implemented to maintain data integrity:

1. **BaseEntity Auditing Trigger** - Automatically updates `CreatedAt` and `UpdatedAt` columns:

```sql
CREATE OR REPLACE FUNCTION update_timestamp_fields()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        NEW."CreatedAt" = CURRENT_TIMESTAMP;
    END IF;
    NEW."UpdatedAt" = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_timestamp_fields_trigger
BEFORE INSERT OR UPDATE ON "Articles"
FOR EACH ROW
EXECUTE FUNCTION update_timestamp_fields();
-- Similar triggers for other tables inheriting from BaseEntity
```

2. **Article Publication Trigger** - Manages the `PublishedAt` timestamp when status changes:

```sql
CREATE OR REPLACE FUNCTION manage_article_publication_dates()
RETURNS TRIGGER AS $$
BEGIN
    -- Set PublishedAt to current time when status changes to Published
    IF NEW."Status" = 'Published' AND (OLD."Status" IS NULL OR OLD."Status" <> 'Published') AND NEW."PublishedAt" IS NULL THEN
        NEW."PublishedAt" = CURRENT_TIMESTAMP;
    END IF;
    -- Clear PublishedAt if changing back to Draft
    IF NEW."Status" = 'Draft' AND OLD."Status" <> 'Draft' THEN
        NEW."PublishedAt" = NULL;
        NEW."PublishedUntil" = NULL;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER manage_article_publication_dates_trigger
BEFORE UPDATE ON "Articles"
FOR EACH ROW
EXECUTE FUNCTION manage_article_publication_dates();
```

### Unique Constraints

In addition to primary keys, several unique constraints are implemented:

1. **Unique article slug**:
```sql
CREATE UNIQUE INDEX "IX_Articles_Slug" ON "Articles" USING btree ("Slug");
```

2. **Unique logical name within article scope**:
```sql
CREATE UNIQUE INDEX "IX_Attachments_ArticleId_LogicalName" ON "Attachments" USING btree ("ArticleId", "LogicalName");
```

### Cascading Delete Behavior

The system uses cascading deletes for certain relationships to maintain data integrity when a parent record is deleted:

1. **Articles and related entities**:
   - When an article is deleted, all associated comments, ratings, and social media posts are automatically deleted.
   - Attachments owned by the article (where OwnerType = "Article" and OwnerId = Article.Id) are also deleted.
   - This is implemented with `ON DELETE CASCADE` constraints and custom delete behavior for polymorphic associations.

2. **Comments with replies**:
   - When a parent comment is deleted, all reply comments are also deleted.

3. **Social Media Accounts**:
   - When a social media account is deleted, associated posts are deleted, but articles are preserved.
   - Attachments owned by the account (where OwnerType = "SocialMediaAccount" and OwnerId = SocialMediaAccount.Id) are also deleted.

4. **Social Media Posts**:
   - When a social media post is deleted, any associated attachments (where OwnerType = "SocialMediaPost" and OwnerId = SocialMediaPost.Id) are also deleted.

5. **Users**:
   - When a user is deleted, their created content (articles, comments) remains but the CreatedById reference is set to NULL.
   - Attachments owned by the user (where OwnerType = "ApplicationUser" and OwnerId = AspNetUsers.Id) are also deleted.
   - This is implemented with `ON DELETE SET NULL` constraints on the CreatedById and UpdatedById columns and custom delete behavior for polymorphic associations.

Example foreign key and trigger definitions:

```sql
-- Traditional foreign key cascade delete
ALTER TABLE "Comments" ADD CONSTRAINT "FK_Comments_Comments_ParentCommentId" 
    FOREIGN KEY ("ParentCommentId") REFERENCES "Comments" ("Id") ON DELETE CASCADE;

ALTER TABLE "BaseEntity" ADD CONSTRAINT "FK_BaseEntity_AspNetUsers_CreatedById" 
    FOREIGN KEY ("CreatedById") REFERENCES "AspNetUsers" ("Id") ON DELETE SET NULL;

-- Trigger for polymorphic cascading delete
CREATE OR REPLACE FUNCTION delete_entity_attachments()
RETURNS TRIGGER AS $$
BEGIN
    -- Delete attachments for the entity being deleted
    DELETE FROM "Attachments" 
    WHERE "OwnerType" = TG_ARGV[0] AND "OwnerId" = OLD."Id";
    
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

-- Create triggers for each entity type that owns attachments
CREATE TRIGGER delete_article_attachments_trigger
AFTER DELETE ON "Articles"
FOR EACH ROW
EXECUTE FUNCTION delete_entity_attachments('Article');

CREATE TRIGGER delete_social_media_post_attachments_trigger
AFTER DELETE ON "SocialMediaPosts"
FOR EACH ROW
EXECUTE FUNCTION delete_entity_attachments('SocialMediaPost');
```

### Polymorphic Attachment Design

The Attachment entity uses a polymorphic association pattern to enable flexible relationships with multiple entity types:

1. **Design Pattern**:
   - Instead of using traditional foreign keys that reference a specific table, the Attachment entity uses a combination of `OwnerId` and `OwnerType` fields.
   - The `OwnerId` stores the ID of the owning entity (e.g., an Article.Id or ApplicationUser.Id).
   - The `OwnerType` stores the type of the owning entity (e.g., "Article", "ApplicationUser", "SocialMediaPost").

2. **Benefits**:
   - Allows a single Attachment table to serve multiple entity types
   - Avoids creating separate attachment tables for each entity type
   - Provides consistent attachment handling across the application
   - Enables centralized attachment storage and management
   - Simplifies queries for finding all attachments regardless of owner type

3. **Implementation in Entity Framework Core**:
   ```csharp
   // In DbContext configuration
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       // Create a composite index for efficient lookups
       modelBuilder.Entity<Attachment>()
           .HasIndex(a => new { a.OwnerType, a.OwnerId });
           
       // Create a unique constraint for logical names within owner scope
       modelBuilder.Entity<Attachment>()
           .HasIndex(a => new { a.OwnerType, a.OwnerId, a.LogicalName })
           .IsUnique();
   }
   ```

4. **Handling Deletion**:
   - EF Core does not natively support cascade delete for polymorphic associations
   - The application implements cascade delete through one of three approaches:
     
     a. **Application-level cascade delete**:
     ```csharp
     // In a service method
     public async Task DeleteEntityAsync<T>(string entityId) where T : BaseEntity
     {
         // Delete the entity
         var entity = await _dbContext.Set<T>().FindAsync(entityId);
         if (entity != null)
         {
             _dbContext.Set<T>().Remove(entity);
             
             // Delete associated attachments
             var attachments = await _dbContext.Attachments
                 .Where(a => a.OwnerType == typeof(T).Name && a.OwnerId == entityId)
                 .ToListAsync();
                 
             _dbContext.Attachments.RemoveRange(attachments);
             
             await _dbContext.SaveChangesAsync();
         }
     }
     ```
     
     b. **Database triggers** (as shown in the SQL example above):
     - Ensures attachments are deleted even if deleted through direct database operations
     - Maintains referential integrity at database level
     
     c. **Domain events pattern** (recommended approach):
     ```csharp
     // 1. Define domain event
     public record EntityDeletedEvent(BaseEntity Entity) : IDomainEvent;
     
     // 2. Create event handler
     public class EntityDeletedEventHandler : INotificationHandler<EntityDeletedEvent>
     {
         private readonly IProPulseDbContext _dbContext;
         
         public EntityDeletedEventHandler(IProPulseDbContext dbContext)
         {
             _dbContext = dbContext;
         }
         
         public async Task Handle(EntityDeletedEvent notification, CancellationToken cancellationToken)
         {
             var entity = notification.Entity;
             
             // Find and delete any attachments owned by this entity
             var attachments = await _dbContext.Attachments
                 .Where(a => a.OwnerType == entity.GetType().Name && a.OwnerId == entity.Id)
                 .ToListAsync(cancellationToken);
                 
             if (attachments.Any())
             {
                 _dbContext.Attachments.RemoveRange(attachments);
                 await _dbContext.SaveChangesAsync(cancellationToken);
             }
         }
     }
     ```

5. **Helper Methods**:
   ```csharp
   // Extension methods for working with attachments
   public static class AttachmentExtensions
   {
       // Get all attachments for an entity
       public static IQueryable<Attachment> GetAttachmentsForEntity<T>(
           this IQueryable<Attachment> query, string entityId) where T : BaseEntity
       {
           return query.Where(a => a.OwnerType == typeof(T).Name && a.OwnerId == entityId);
       }
       
       // Add an attachment to an entity
       public static Attachment AddAttachmentToEntity<T>(
           this T entity, string logicalName, string contentType, string storageLocation, long fileSize)
           where T : BaseEntity
       {
           return new Attachment
           {
               OwnerId = entity.Id,
               OwnerType = typeof(T).Name,
               LogicalName = logicalName,
               ContentType = contentType,
               StorageLocation = storageLocation,
               FileSize = fileSize
           };
       }
   }
   ```

The polymorphic association pattern provides a more flexible approach to managing attachments across different entity types while maintaining data integrity. This approach aligns with modern database design practices and enables efficient querying and management of attachments regardless of which entity type they belong to.

## 3.4. Data Classification and Security

### 3.4.1. PII and Sensitive Data Classification

The ProPulse data model includes several categories of personal and sensitive information that require special handling:

| Entity | Field | Classification | Sensitivity | Encryption | Purpose |
|--------|-------|---------------|-------------|------------|---------|
| AspNetUsers | Id | Identifier | Low | No | User identification |
| AspNetUsers | UserName | Direct PII | Medium | No | Login, display |
| AspNetUsers | Email | Direct PII | High | No | Communication |
| AspNetUsers | PasswordHash | Authentication | Critical | Yes (hashing) | Authentication |
| AspNetUsers | DisplayName | Direct PII | Medium | No | Public display |
| AspNetUsers | ProfilePictureUrl | Indirect PII | Low | No | Public display |
| AspNetUsers | Bio | Indirect PII | Low | No | Public display |
| SocialMediaAccount | AccessToken | Credentials | Critical | Yes (column-level) | API access |
| SocialMediaAccount | RefreshToken | Credentials | Critical | Yes (column-level) | Token renewal |
| BaseEntity | CreatedById | Association | Low | No | Audit trail |
| Article | Content | User Content | Low | No | Publication |
| Comment | Content | User Content | Low | No | Interaction |

### 3.4.2. Data Encryption Implementation

ProPulse implements several encryption mechanisms:

1. **Column-Level Encryption**:
   ```csharp
   // Implementation pattern for encrypting social media tokens
   public class SocialMediaAccount : BaseEntity
   {
       // Stored encrypted in the database
       public string AccessToken { get; set; }
       
       // Non-mapped property to access decrypted value
       [NotMapped]
       public string DecryptedAccessToken
       {
           get => EncryptionService.Decrypt(AccessToken);
           set => AccessToken = EncryptionService.Encrypt(value);
       }
   }
   ```

2. **Entity Framework Value Converters**:
   ```csharp
   // In DbContext configuration
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       // Configure encrypted fields using value converters
       modelBuilder.Entity<SocialMediaAccount>()
           .Property(e => e.AccessToken)
           .HasConversion(
               v => EncryptionService.Encrypt(v),
               v => EncryptionService.Decrypt(v));
             
       // Similar configuration for other sensitive fields
   }
   ```

3. **Encryption Service**:
   The system uses Azure Key Vault for key management and AES-256 for encryption.

### 3.4.3. Audit Trail Implementation

The BaseEntity includes audit fields, but the system implements additional audit capabilities:

1. **Entity History Tracking**:
   - Entity changes tracked in separate history tables
   - Implemented through triggers or EF Core interceptors
   - Maintains immutable record of all data changes

2. **User Action Audit**:
   ```sql
   CREATE TABLE "AuditLogs" (
       "Id" text NOT NULL,
       "UserId" text NULL,
       "Action" text NOT NULL,
       "EntityType" text NOT NULL,
       "EntityId" text NOT NULL,
       "Timestamp" timestamptz NOT NULL,
       "OldValues" jsonb NULL,
       "NewValues" jsonb NULL,
       "ClientIP" text NULL,
       CONSTRAINT "PK_AuditLogs" PRIMARY KEY ("Id")
   );
   ```

3. **MediatR Audit Behavior**:
   - Cross-cutting audit behavior for all commands
   - Captures pre and post operation state
   - Automatically logs to audit trail
