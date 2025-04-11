using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProPulse.Domain.Entities.Identity;

namespace ProPulse.Infrastructure.Data.Configurations;

/// <summary>
/// Entity type configuration for the ApplicationUser entity
/// </summary>
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    /// <summary>
    /// Configures the entity type
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context</param>
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Set the schema
        builder.ToTable("AspNetUsers", "identity");

        // Configure properties
        builder.Property(u => u.DisplayName)
            .HasMaxLength(100);

        builder.Property(u => u.Bio)
            .HasMaxLength(2000);

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Configure indexes
        builder.HasIndex(u => u.NormalizedUserName).IsUnique();
        builder.HasIndex(u => u.NormalizedEmail).IsUnique();
        builder.HasIndex(u => u.IsDeleted);
        builder.HasIndex(u => u.CreatedAt);

        // Add global query filter for soft delete
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
