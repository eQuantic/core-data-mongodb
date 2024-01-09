using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using eQuantic.Linq.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace eQuantic.Core.Data.MongoDb.Repository.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Include property
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="property">The property.</param>
    /// <param name="database">The database.</param>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> query, IMongoDatabase database, string property)
        where TEntity : class
    {
        if (query is not IMongoQueryable<TEntity> mongoQuery) 
            return query;
        
        var properties = GetProperties<TEntity>(property);

        var currentType = typeof(TEntity);
        foreach (var propertyInfo in properties)
        {
            var propKeyInfo = currentType.GetForeignKey(propertyInfo.Name);
            var propKeyExp = propKeyInfo?.Name.GetColumnExpression(currentType);
            var keyExp = propertyInfo.PropertyType.GetPrimaryKey()?.Name.GetColumnExpression(propertyInfo.PropertyType);
            
            currentType = propertyInfo.PropertyType;
            
            if(propKeyInfo == null)
                continue;
            
            var collection = database.GetCollection(propertyInfo.PropertyType);
            var joinMethod = typeof(MongoQueryable).GetMethod(nameof(MongoQueryable.Join));
            
            if (joinMethod == null) 
                continue;
            
            query = joinMethod
                .Invoke(null, new object[]
                {
                    mongoQuery, 
                    collection, 
                    propKeyExp,
                    keyExp
                }) as IQueryable<TEntity>;
            return query;
        }
        return mongoQuery;
    }

    /// <summary>
    /// Include property
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="database">The database.</param>
    /// <param name="property">The property.</param>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> query, IMongoDatabase database,  Expression<Func<TEntity, object>> property)
        where TEntity : class
    {
        return Include(query, database, property.GetColumnName());
    }

    /// <summary>
    /// Include many properties.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="query">The query.</param>
    /// <param name="database">The database.</param>
    /// <param name="properties">The properties.</param>
    /// <returns></returns>
    public static IQueryable<TEntity> IncludeMany<TEntity>(this IQueryable<TEntity> query, IMongoDatabase database, params string[] properties)
        where TEntity : class
    {
        if (properties is { Length: > 0 })
        {
            query = properties.Where(property => !string.IsNullOrEmpty(property))
                .Aggregate(query, (current, property) => current.Include(database, property));
        }

        return query;
    }

    /// <summary>
    /// Include many properties.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="query">The query.</param>
    /// <param name="database">The database.</param>
    /// <param name="properties">The properties.</param>
    /// <returns></returns>
    public static IQueryable<TEntity> IncludeMany<TEntity>(this IQueryable<TEntity> query,
        IMongoDatabase database, params Expression<Func<TEntity, object>>[] properties)
        where TEntity : class
    {
        if (properties != null && properties.Length > 0)
        {
            query = properties.Where(property => property != null)
                .Aggregate(query, (current, property) => current.Include(database, property));
        }

        return query;
    }
    
    public static List<PropertyInfo> GetProperties<T>(string propertyName, bool useColumnFallback = false)
    {
        var properties = new List<PropertyInfo>();

        var declaringType = typeof(T);

        foreach (var name in propertyName.Split('.'))
        {
            var property = GetPropertyByName(propertyName, declaringType, name, useColumnFallback);

            properties.Add(property);

            declaringType = property.PropertyType;
        }

        return properties;
    }

    private static PropertyInfo GetPropertyByName(string propertyName, Type declaringType, string subPropertyName, bool useColumnFallback = false)
    {
        const BindingFlags flags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;
        var genericType = declaringType.GenericTypeArguments.FirstOrDefault();
        var prop =  (genericType != null && declaringType.GetGenericTypeDefinition().GetInterfaces().Any(o => o == typeof(IEnumerable)) ?
            genericType : declaringType).GetProperty(subPropertyName, flags);
        if (prop != null)
            return prop;
        
        if (useColumnFallback)
        {
            var propertiesWithColumns = declaringType.GetProperties(flags)
                .Where(p => p.IsDefined(typeof(ColumnAttribute), false));
            foreach (var propertyInfo in propertiesWithColumns)
            {
                if (propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name == subPropertyName)
                {
                    return propertyInfo;
                }
            }
        }

        throw new ArgumentException($"{propertyName} could not be parsed. {declaringType} does not contain a property named '{subPropertyName}'.", nameof(propertyName));

    }
}