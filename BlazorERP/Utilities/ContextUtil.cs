using System.Linq.Expressions;
using BlazorERP.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BlazorERP.Utilities;

public static class ContextUtil
{
    /// <summary>
    /// Checks if the given value matches any other values in a specified column in a database table.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="entity"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool ValueExists<T, TProperty>(this proContext context, Expression<Func<T, TProperty>> propExpression, TProperty value) where T : class
    {
        // Create a new expression that represents the property access
        var body = Expression.Equal(
            propExpression.Body,
            Expression.Constant(value, typeof(TProperty))
            );
        
        // Create a new lambda expression that represents the condition
        var lambda = Expression.Lambda<Func<T, bool>>(body, propExpression.Parameters);
        
        return context.Set<T>().Any(lambda);
    }

    /// <summary>
    /// Loads a collection of related entities for a given entity, applying a filter predicate.
    /// </summary>
    /// <param name="context"> Database Context </param>
    /// <param name="entity"> Entity to load related entities for </param>
    /// <param name="navigation"> Related entity to load /param>
    /// <param name="filter">Entity where clause</param>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TRelated"></typeparam>
    public static async Task LoadCollectionsAsync<TEntity, TRelated>(
        this DbContext context,
        TEntity entity,
        Expression<Func<TEntity, IEnumerable<TRelated>>> navigation,
        Expression<Func<TRelated, bool>> filter)
        where TEntity : class
        where TRelated : class
    {
        // Convert nav to CollectionEntry<TEntity, TRelated>
        var collection = context
            .Entry(entity)
            .Collection(navigation);
        
        // Apply filter and load data
        await collection
            .Query()
            .Where(filter)
            .LoadAsync()
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Loads a reference to a related entity for a given entity.
    /// </summary>
    /// <param name="context"> Database Context </param>
    /// <param name="entity"> Entity to load reference for</param>
    /// <param name="navigation"> Navigation to reference</param>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TRelated"></typeparam>
    public static async Task LoadReferenceAsync<TEntity, TRelated>(
        this DbContext                     context,
        TEntity                            entity,
        Expression<Func<TEntity, TRelated>> navigation)
        where TEntity  : class
        where TRelated : class
    {
        await context.Entry(entity)
            .Reference(navigation)
            .LoadAsync()
            .ConfigureAwait(false);
    }
    
    public static async Task GetStatusesAsync<TEntity>(
        this DbContext context,
        TEntity entity,
        Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        // Load the entity with the specified predicate
        var entry = context.Entry(entity);
        
        if (entry.State == EntityState.Detached)
        {
            await context.Set<TEntity>()
                .Where(predicate)
                .LoadAsync()
                .ConfigureAwait(false);
        }
    }

}