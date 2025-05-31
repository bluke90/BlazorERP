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

}