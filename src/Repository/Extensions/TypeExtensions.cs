using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization.Attributes;

namespace eQuantic.Core.Data.MongoDb.Repository.Extensions;

public static class TypeExtensions
{
    public static PropertyInfo GetForeignKey(this Type type, string propertyName)
    {
        return type.GetProperties()
            .FirstOrDefault(p =>
                p.GetCustomAttribute<ForeignKeyAttribute>() != null &&
                p.GetCustomAttribute<ForeignKeyAttribute>().Name.Equals(propertyName,
                    StringComparison.InvariantCultureIgnoreCase));
    }
    
    public static PropertyInfo GetPrimaryKey(this Type type)
    {
        return type.GetProperties()
            .FirstOrDefault(p => 
                p.GetCustomAttribute<KeyAttribute>() != null || 
                p.GetCustomAttribute<BsonIdAttribute>() != null);
    }
}