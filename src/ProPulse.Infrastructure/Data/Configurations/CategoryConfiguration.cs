using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProPulse.Domain.Entities;

namespace ProPulse.Infrastructure.Data.Configurations;

/// <summary>
/// Entity type configuration for the Category entity
/// </summary>
public class CategoryConfiguration : BaseEntityTypeConfiguration<Category>
{
    /// <summary>
    /// Configures the entity type
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context</param>
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);

        // Set the schema
        builder.ToTable("Categories", "propulse");

        // Configure properties
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        // Configure relationships
        builder.HasMany(c => c.Articles)
            .WithOne(a => a.Category)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure indexes
        builder.HasIndex(c => c.Name)
            .IsUnique();
        builder.HasIndex(c => c.Slug)
            .IsUnique();
    }
}
