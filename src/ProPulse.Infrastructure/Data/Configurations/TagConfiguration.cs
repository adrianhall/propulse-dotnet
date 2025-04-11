using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProPulse.Domain.Entities;

namespace ProPulse.Infrastructure.Data.Configurations;

/// <summary>
/// Entity type configuration for the Tag entity
/// </summary>
public class TagConfiguration : BaseEntityTypeConfiguration<Tag>
{
    /// <summary>
    /// Configures the entity type
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context</param>
    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        base.Configure(builder);

        // Set the schema
        builder.ToTable("Tags", "propulse");

        // Configure properties
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(50);

        // Configure relationships
        builder.HasMany(t => t.Articles)
            .WithMany(a => a.Tags)
            .UsingEntity<Dictionary<string, object>>(
                "ArticleTag",
                j => j.HasOne<Article>().WithMany().HasForeignKey("ArticleId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("ArticleTag", "propulse");
                    j.HasKey("ArticleId", "TagId");
                });

        // Configure indexes
        builder.HasIndex(t => t.Name)
            .IsUnique();
        builder.HasIndex(t => t.Slug)
            .IsUnique();
    }
}
