using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProPulse.Domain.Entities;

namespace ProPulse.Infrastructure.Data.Configurations;

/// <summary>
/// Entity type configuration for the Bookmark entity
/// </summary>
public class BookmarkConfiguration : BaseEntityTypeConfiguration<Bookmark>
{
    /// <summary>
    /// Configures the entity type
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context</param>
    public override void Configure(EntityTypeBuilder<Bookmark> builder)
    {
        base.Configure(builder);

        // Set the schema
        builder.ToTable("Bookmarks", "propulse");

        // Configure relationships
        builder.HasOne(b => b.Article)
            .WithMany()
            .HasForeignKey(b => b.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.HasIndex(b => b.ArticleId);
        builder.HasIndex(b => b.CreatedBy);

        // User-specific access optimization
        builder.HasIndex(b => new { b.CreatedBy, b.ArticleId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
