using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProPulse.Domain.Entities;

namespace ProPulse.Infrastructure.Data.Configurations;

/// <summary>
/// Entity type configuration for the ReadingHistory entity
/// </summary>
public class ReadingHistoryConfiguration : BaseEntityTypeConfiguration<ReadingHistory>
{
    /// <summary>
    /// Configures the entity type
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context</param>
    public override void Configure(EntityTypeBuilder<ReadingHistory> builder)
    {
        base.Configure(builder);

        // Set the schema
        builder.ToTable("ReadingHistory", "propulse");

        // Configure relationships
        builder.HasOne(rh => rh.Article)
            .WithMany()
            .HasForeignKey(rh => rh.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.HasIndex(rh => rh.ArticleId);
        builder.HasIndex(rh => rh.CreatedBy);
        builder.HasIndex(rh => rh.CreatedAt);

        // User-specific access optimization (user reading an article)
        builder.HasIndex(rh => new { rh.CreatedBy, rh.ArticleId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
