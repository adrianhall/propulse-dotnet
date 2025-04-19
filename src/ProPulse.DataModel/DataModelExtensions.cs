using Microsoft.EntityFrameworkCore;
using ProPulse.Core.Entities;

namespace ProPulse.DataModel;

/// <summary>
/// A set of helper extensions for the data model.
/// </summary>
public static class DataModelExtensions
{
    /// <summary>
    /// A helper method for applying global query filters to an IQueryable.
    /// </summary>
    /// <typeparam name="T">The type of the entity being searched</typeparam>
    /// <param name="query">The source query</param>
    /// <param name="includeDeleted">Whether to include deleted items.</param>
    /// <returns>The modified query.</returns>
    public static IQueryable<T> IncludingDeletedEntities<T>(this IQueryable<T> query, bool includeDeleted) where T : BaseEntity
        => includeDeleted ? query.IgnoreQueryFilters() : query;
}