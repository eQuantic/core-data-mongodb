using System;
using MongoDB.Driver;

namespace eQuantic.Core.Data.MongoDb.Repository.Extensions;

public static class MongoDatabaseExtensions
{
    public static object GetCollection(this IMongoDatabase database, Type type)
    {
        return typeof(IMongoDatabase)
            .GetMethod(nameof(IMongoDatabase.GetCollection), 1, new[] { type })
            .Invoke(database, new object[] { type.Name });
    }
}