using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Extensions;
using Humanizer;
using MongoDB.Driver;

namespace eQuantic.Core.Data.MongoDb.Repository.Extensions;

public static class MongoDatabaseExtensions
{
    private const string DefaultEntitySuffix = "Data";

    public static IMongoCollection<TEntity> GetCollection<TEntity>(this IMongoDatabase database) where TEntity : IEntity
    {
        return database.GetCollection<TEntity>(GetCollectionName(typeof(TEntity)));
    }

    public static object GetCollection(this IMongoDatabase database, Type type)
    {
        var getCollectionMethod = typeof(MongoDatabaseExtensions)
            .GetMethods()
            .FirstOrDefault(m => m.Name.Equals(nameof(GetCollection)) && m.IsGenericMethod)?
            .MakeGenericMethod(type);
        return getCollectionMethod?.Invoke(null, new object[] { database });
    }

    private static string GetCollectionName(MemberInfo type)
    {
        var tableAttr = type.GetCustomAttribute<TableAttribute>();
        var collectionName = type.Name.TrimEnd(DefaultEntitySuffix).Pluralize();
        if (tableAttr != null)
            collectionName = tableAttr.Name;

        return collectionName;
    }
}