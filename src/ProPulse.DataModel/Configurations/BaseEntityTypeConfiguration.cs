using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProPulse.Core;
using ProPulse.Core.Entities;
using ProPulse.DataModel.ValueConverters;

namespace ProPulse.DataModel.Configurations;

/// <summary>
/// Provides a base entity type configuration for all entities derived from <see cref="BaseEntity"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public abstract class BaseEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    /// <inheritdoc />
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // All entities have an ID that is a uuid type.
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .IsRequired()
            .HasColumnType("uuid")
            .HasMaxLength(AppConstants.IdMaxLength)
            .ValueGeneratedOnAdd();

        // The CreatedAt property is a timestamp and maintained by the server.
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now() at time zone 'utc'")
            .ValueGeneratedOnAdd();

        // The UpdatedAt property is a timestamp and maintained by the server.
        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now() at time zone 'utc'")
            .ValueGeneratedOnAddOrUpdate();

        // The DeletedAt property is a timestamp and maintained by the server.
        builder.Property(e => e.DeletedAt)
            .HasColumnType("timestamptz")
            .ValueGeneratedOnUpdate();

        // The Version property maps to the PostgreSQL xmin system column for concurrency control.
        builder.Property(e => e.Version)
            .IsRequired()
            .HasColumnType("xid")
            .HasColumnName("xmin")
            .HasConversion(new XminValueConverter())
            .ValueGeneratedOnAddOrUpdate();

        // The IsDeleted property is used for soft-delete filtering.
        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasColumnType("boolean")
            .HasDefaultValue(false);

        builder.HasIndex(e => e.CreatedAt).HasDatabaseName($"IDX_{typeof(TEntity).Name}s_CreatedAt");
        builder.HasIndex(e => e.UpdatedAt).HasDatabaseName($"IDX_{typeof(TEntity).Name}s_UpdatedAt");
        builder.HasIndex(e => e.DeletedAt).HasDatabaseName($"IDX_{typeof(TEntity).Name}s_DeletedAt");

        // Global query filter to exclude soft-deleted entities by default.
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
