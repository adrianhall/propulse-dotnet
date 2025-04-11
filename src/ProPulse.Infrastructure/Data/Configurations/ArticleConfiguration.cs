using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProPulse.Domain.Entities;

namespace ProPulse.Infrastructure.Data.Configurations;

/// <summary>
/// Entity type configuration for the Article entity
/// </summary>
public class ArticleConfiguration : BaseEntityTypeConfiguration<Article>
{
    /// <summary>
    /// Configures the entity type
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context</param>
    public override void Configure(EntityTypeBuilder<Article> builder)
    {
        base.Configure(builder);

        // Set the schema
        builder.ToTable("Articles", "propulse");

        // Configure properties
        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.Slug)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.Content)
            .IsRequired();

        builder.Property(a => a.Excerpt)
            .HasMaxLength(4096);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ArticleStatus.Draft);

        builder.Property(a => a.ViewCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Configure relationships
        builder.HasMany(a => a.Attachments)
            .WithOne(a => a.Article)
            .HasForeignKey(a => a.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Category)
            .WithMany(c => c.Articles)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Tags)
            .WithMany(t => t.Articles)
            .UsingEntity<Dictionary<string, object>>(
                "ArticleTag",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Article>().WithMany().HasForeignKey("ArticleId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("ArticleTag", "propulse");
                    j.HasKey("ArticleId", "TagId");
                });

        builder.HasMany(a => a.Bookmarks)
            .WithOne(b => b.Article)
            .HasForeignKey(b => b.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.ReadingHistories)
            .WithOne(rh => rh.Article)
            .HasForeignKey(rh => rh.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.HasIndex(a => a.Slug).IsUnique();
        builder.HasIndex(a => a.Title);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.CategoryId);
        builder.HasIndex(a => a.CreatedBy);
        builder.HasIndex(a => a.PublishedAt);
    }
}
