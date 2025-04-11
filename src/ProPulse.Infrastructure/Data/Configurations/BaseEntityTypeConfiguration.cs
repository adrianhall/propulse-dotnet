using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProPulse.Domain.Entities;
using ProPulse.Domain.Entities.Identity;

namespace ProPulse.Infrastructure.Data.Configurations;

/// <summary>
/// Base configuration for all entity types that inherit from BaseEntity
/// </summary>
/// <typeparam name="TEntity">The entity type being configured</typeparam>
public abstract class BaseEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    /// <summary>
    /// Configures the entity type
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context</param>
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Set the primary key
        builder.HasKey(e => e.Id);

        // CreatedAt is set via a trigger, with a default value of the current timestamp
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        // UpdatedAt is set via a trigger, with a default value of the current timestamp
        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnUpdate();

        // IsDeleted is set to false by default
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // DeletedAt is set via a trigger, with a default value of null
        builder.Property(e => e.DeletedAt)
            .HasDefaultValue(null)
            .ValueGeneratedOnAddOrUpdate();

        // Configure relationships for CreatedBy and UpdatedBy with ApplicationUser
        builder.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(e => e.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Add global query filter for soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
