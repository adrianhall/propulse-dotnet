using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProPulse.Core.Entities;

namespace ProPulse.DataModel.Configurations;

/// <summary>
/// Configures the <see cref="Article"/> entity for EF Core.
/// </summary>
public class ArticleEntityTypeConfiguration : BaseEntityTypeConfiguration<Article>
{
    /// <inheritdoc />
    public override void Configure(EntityTypeBuilder<Article> builder)
    {
        // Sets up common properties for all entities derived from BaseEntity.
        base.Configure(builder);

        // The Articles table is in the "propulse" schema.
        builder.ToTable("Articles", schema: "propulse");

        // Configure individual properties for the Article entity.
        builder.Property(a => a.Title)
            .HasColumnType("citext")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(a => a.Content)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(a => a.PublishedAt)
            .HasColumnType("timestamptz");
    }
}
