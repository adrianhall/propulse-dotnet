using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProPulse.Domain.Entities;

namespace ProPulse.Infrastructure.Data.Configurations;

/// <summary>
/// Entity type configuration for the Attachment entity
/// </summary>
public class AttachmentConfiguration : BaseEntityTypeConfiguration<Attachment>
{
    /// <summary>
    /// Configures the entity type
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context</param>
    public override void Configure(EntityTypeBuilder<Attachment> builder)
    {
        base.Configure(builder);

        // Set the schema
        builder.ToTable("Attachments", "propulse");

        // Configure properties
        builder.Property(a => a.SymbolicName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.StorageLocation)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.FileSize)
            .IsRequired();

        builder.Property(a => a.AttachmentType)
            .HasConversion<string>();

        // Configure relationships
        builder.HasOne(a => a.Article)
            .WithMany(article => article.Attachments)
            .HasForeignKey(a => a.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.HasIndex(a => a.ArticleId);
        builder.HasIndex(a => a.SymbolicName);
        builder.HasIndex(a => a.ContentType);
        builder.HasIndex(a => a.AttachmentType);
    }
}
