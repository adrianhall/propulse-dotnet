using Microsoft.EntityFrameworkCore;
using ProPulse.Core.Entities;
using ProPulse.DataModel.Configurations;

namespace ProPulse.DataModel;

/// <summary>
/// Represents the application's Entity Framework Core database context.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the articles in the database.
    /// </summary>
    public DbSet<Article> Articles => Set<Article>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("citext");
        modelBuilder.HasPostgresExtension("uuid-ossp");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
