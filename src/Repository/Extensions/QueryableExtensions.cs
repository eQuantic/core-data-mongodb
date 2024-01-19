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
            var type = typeof(TEntity);
            var propKeyInfo = currentType.GetForeignKey(propertyInfo.Name);
            var propKeyExp = propKeyInfo?.Name.GetColumnExpression(currentType);
            var primaryKey = propertyInfo.PropertyType.GetPrimaryKey();
            var keyExp = primaryKey?.Name.GetColumnExpression(propertyInfo.PropertyType);
            
            currentType = propertyInfo.PropertyType;
            
            if(propKeyInfo == null)
                continue;
            
            var collection = database.GetCollection(propertyInfo.PropertyType);
            var joinMethod = typeof(MongoQueryable)
                .GetMethods()
                .FirstOrDefault (m => 
                    m.Name.Equals(nameof(MongoQueryable.Join)) && 
                    m.GetParameters().Any(p => p.ParameterType.Name == typeof(IMongoCollection<>).Name))?
                .MakeGenericMethod(type, propertyInfo.PropertyType, propKeyInfo.PropertyType, typeof(TEntity));
            
            if (joinMethod == null) 
                continue;

            var resultExp = MappingDynamicFunc(typeof(TEntity), propertyInfo.PropertyType, propertyInfo.Name);
            
            query = joinMethod
                .Invoke(null, new object[]
                {
                    mongoQuery, 
                    collection, 
                    propKeyExp,
                    keyExp,
                    resultExp
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

    private static Expression MappingDynamicFunc(Type firstType, Type secondType, string propertyName)
    {
        var method = typeof(QueryableExtensions)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .FirstOrDefault(m => m.Name.Equals(nameof(MappingDynamicFunc)) && m.IsGenericMethod)?
            .MakeGenericMethod(firstType, secondType);
        return (Expression)method?.Invoke(null, new object[] { propertyName });
    }
    
    private static Expression<Func<TFirst, TSecond, TFirst>> MappingDynamicFunc<TFirst, TSecond>(string propertyName)
    {
        var typeFirst = typeof(TFirst);
        var paramFirst = Expression.Parameter(typeFirst, "paramFirst");
        var paramSecond = Expression.Parameter(typeof(TSecond), "paramSecond");
        var ctor = Expression.New(typeFirst);
        var propertyInfos = typeFirst.GetProperties();
        var objInit = Expression
            .MemberInit(ctor, propertyInfos.Select(pi =>
                Expression.Bind(pi,pi.Name == propertyName ? paramSecond : Expression.Property(paramFirst, pi.Name))));
        
        return Expression.Lambda<Func<TFirst, TSecond, TFirst>>(objInit, new ParameterExpression[] { paramFirst, paramSecond });
    }
}