using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using eQuantic.Core.Data.MongoDb.Repository.Extensions;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Config;
using eQuantic.Core.Extensions;
using eQuantic.Linq.Extensions;
using Humanizer;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eQuantic.Core.Data.MongoDb.Repository;

public class Set<TEntity> : Data.Repository.ISet<TEntity>, IMongoQueryable<TEntity>
    where TEntity : class, IEntity, new()
{
    private readonly IMongoDatabase _database;
    private const string IdKey = "_id";
    private readonly IMongoCollection<TEntity> _collection;
    private readonly IMongoQueryable<TEntity> _internalQueryable;
    private const string DefaultEntitySuffix = "Data";
    
    public Set(IMongoDatabase database)
    {
        _database = database;

        var type = typeof(TEntity);
        var tableAttr = type.GetCustomAttribute<TableAttribute>();
        var collectionName = type.Name.TrimEnd(DefaultEntitySuffix).Pluralize();
        if (tableAttr != null)
            collectionName = tableAttr.Name;
        
        _collection = database.GetCollection<TEntity>(collectionName);
        _internalQueryable = _collection.AsQueryable();
    }

    public Type ElementType => _internalQueryable.ElementType;

    public Expression Expression => _internalQueryable.Expression;

    public IQueryProvider Provider => _internalQueryable.Provider;

    public void Delete(Expression<Func<TEntity, bool>> filter)
    {
        this._collection.DeleteOne(filter);
    }

    public Task DeleteAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return this._collection.DeleteOneAsync(filter, cancellationToken);
    }

    public long DeleteMany(Expression<Func<TEntity, bool>> filter)
    {
        var result = this._collection.DeleteMany(filter);
        return result.DeletedCount;
    }

    public async Task<long> DeleteManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        var result = await this._collection.DeleteManyAsync(filter, cancellationToken);
        return result.DeletedCount;
    }

    public IEnumerable<TEntity> Execute()
    {
        return _internalQueryable.ToList();
    }

    public IFindFluent<TEntity, TEntity> Find(Expression<Func<TEntity, bool>> filter)
    {
        return this._collection.Find(filter);
    }

    public TEntity Find<TKey>(TKey key)
    {
        //if (Convert.GetTypeCode(key) == TypeCode.Object)
        //{
        //    var expression = GetKeyExpression(key);
        //    return this.collection.Find(expression).FirstOrDefault();
        //}
        var filter = Builders<TEntity>.Filter.Eq(IdKey, ParseKeyValue(key));
        return this._collection.Find(filter).FirstOrDefault();
    }

    public async Task<TEntity> FindAsync<TKey>(TKey key, CancellationToken cancellationToken = default)
    {
        //IAsyncCursor<TEntity> cursor;
        //if (Convert.GetTypeCode(key) == TypeCode.Object)
        //{
        //    var expression = GetKeyExpression(key);
        //    cursor = await this.collection.FindAsync(expression, null, cancellationToken);
        //    return cursor.FirstOrDefault();
        //}
        var filter = Builders<TEntity>.Filter.Eq(IdKey, ParseKeyValue(key));
        var cursor = await this._collection.FindAsync(filter, null, cancellationToken);
        return cursor.FirstOrDefault();
    }

    public IEnumerator<TEntity> GetEnumerator()
    {
        return _internalQueryable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _internalQueryable.GetEnumerator();
    }

    public QueryableExecutionModel GetExecutionModel()
    {
        return _internalQueryable.GetExecutionModel();
    }

    public virtual Expression<Func<TEntity, bool>> GetKeyExpression<TKey>(TKey key)
    {
        Expression<Func<TEntity, bool>> exp = null;

        var values = GetKeyDictionary(key);
        var arg = Expression.Parameter(typeof(TEntity), "item");
        foreach (var kv in values)
        {
            var property = Expression.Property(arg, kv.Key);
            var constant = Expression.Constant(kv.Value);
            var compare = Expression.Equal(property, constant);
            if (exp == null)
                exp = Expression.Lambda<Func<TEntity, bool>>(compare, arg);
            else exp = Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(exp.Body, compare), arg);
        }

        return exp;
    }

    public virtual TKey GetKeyValue<TKey>(TEntity item)
    {
        var keys = GetKeys<TKey>();
        var values = new Dictionary<string, object>();
        if (keys.Any())
        {
            var itemType = typeof(TEntity);
            foreach (var key in keys)
            {
                var prop = itemType.GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (prop == null) continue;

                values.Add(key, prop.GetValue(item));
            }
        }

        if (values.Count > 1) return GetKeyObject<TKey>(values);
        return (TKey)values.FirstOrDefault().Value;
    }

    public void Insert(TEntity item)
    {
        this._collection.InsertOne(item);
    }

    public Task InsertAsync(TEntity item, CancellationToken cancellationToken = default)
    {
        return this._collection.InsertOneAsync(item, null, cancellationToken);
    }

    public IAsyncCursor<TEntity> ToCursor(CancellationToken cancellationToken = default)
    {
        return _internalQueryable.ToCursor(cancellationToken);
    }

    public Task<IAsyncCursor<TEntity>> ToCursorAsync(CancellationToken cancellationToken = default)
    {
        return _internalQueryable.ToCursorAsync(cancellationToken);
    }

    public long Update(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity>> updateExpression)
    {
        var updateDefinition = GetUpdateDefinition(updateExpression.Body);
        var result = this._collection.UpdateOne<TEntity>(filter, updateDefinition);
        return result.ModifiedCount;
    }

    public async Task<long> UpdateAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity>> updateExpression, CancellationToken cancellationToken = default)
    {
        var updateDefinition = GetUpdateDefinition(updateExpression.Body);
        var result = await this._collection.UpdateOneAsync<TEntity>(filter, updateDefinition, null, cancellationToken);
        return result.ModifiedCount;
    }

    public long UpdateMany(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> updateExpression)
    {
        var updateDefinition = GetUpdateDefinition(updateExpression.Body);
        var result = this._collection.UpdateMany(filter, updateDefinition);
        return result.ModifiedCount;
    }

    public async Task<long> UpdateManyAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> updateExpression, CancellationToken cancellationToken = default)
    {
        var updateDefinition = GetUpdateDefinition(updateExpression.Body);
        var result = await this._collection.UpdateManyAsync(filter, updateDefinition, null, cancellationToken);
        return result.ModifiedCount;
    }

    private static void SetDictionaryValuesFromJToken(Dictionary<string, object> dict, JToken token, string prefix, bool explodeArray = true)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                foreach (JProperty prop in token.Children<JProperty>())
                {
                    SetDictionaryValuesFromJToken(dict, prop.Value, string.IsNullOrEmpty(prefix) ? prop.Name : prefix + "." + prop.Name, explodeArray);
                }
                break;

            case JTokenType.Array:
                int index = 0;
                if (explodeArray)
                {
                    foreach (JToken value in token.Children())
                    {
                        SetDictionaryValuesFromJToken(dict, value, $"{prefix}[{index}]", explodeArray);
                        index++;
                    }
                }
                else
                {
                    dict.Add(prefix, string.Join(",", token.Children().Select(v => ((JValue)v).Value)));
                }
                break;

            default:
                dict.Add(prefix, ((JValue)token).Value);
                break;
        }
    }

    private BsonClassMap GetClassMap()
    {
        return BsonClassMap.GetRegisteredClassMaps().FirstOrDefault(o => o.ClassType == typeof(TEntity));
    }

    private Dictionary<string, object> GetDictionaryFromExpression(Expression expression)
    {
        var dictionary = new Dictionary<string, object>();
        SetDictionaryValuesFromExpression(dictionary, expression);
        return dictionary;
    }

    private Dictionary<string, object> GetDictionaryFromObject(object parameters, string prefix = "")
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        JToken token = JToken.Parse(JsonConvert.SerializeObject(parameters));
        SetDictionaryValuesFromJToken(dict, token, prefix);
        return dict;
    }

    private Dictionary<string, object> GetKeyDictionary<TKey>(TKey key)
    {
        var keyType = typeof(TKey);
        var dict = new Dictionary<string, object>();
        var props = keyType.GetProperties();
        foreach (var prop in props)
        {
            var value = prop.GetValue(key);
            dict.Add(prop.Name, value);
        }
        return dict;
    }

    private T GetKeyObject<T>(Dictionary<string, object> dict)
    {
        Type type = typeof(T);
        var obj = Activator.CreateInstance(type);

        foreach (var kv in dict)
        {
            type.GetProperty(kv.Key).SetValue(obj, kv.Value);
        }
        return (T)obj;
    }

    private IEnumerable<string> GetKeys<TKey>()
    {
        var keyType = typeof(TKey);

        if (Type.GetTypeCode(keyType) == TypeCode.Object)
        {
            var props = keyType.GetProperties();
            return props.Select(p => p.Name);
        }

        return new[] { GetClassMap()?.IdMemberMap?.MemberInfo?.Name ?? IdKey };
    }

    private UpdateDefinition<TEntity> GetUpdateDefinition(Expression updateExpression)
    {
        var values = GetDictionaryFromExpression(updateExpression);

        UpdateDefinition<TEntity> updateDefinition = null;
        var updateBuilder = Builders<TEntity>.Update;

        foreach (var kvp in values)
        {
            if (updateDefinition == null)
                updateDefinition = updateBuilder.Set(kvp.Key, kvp.Value);
            else updateDefinition.Set(kvp.Key, kvp.Value);
        }
        return updateDefinition;
    }

    private object ParseKeyValue<TKey>(TKey key)
    {
        var classMap = GetClassMap();

        if (classMap == null)
        {
            return key;
        }

        object value = key;

        if (Convert.GetTypeCode(key) != TypeCode.Object)
        {
            var memberType = classMap.IdMemberMap?.MemberType;
            if (memberType != null && memberType != typeof(TKey))
            {
                if (memberType == typeof(ObjectId))
                {
                    value = ObjectId.Parse(key.ToString());
                }

                if (memberType == typeof(Guid))
                {
                    value = Guid.Parse(key.ToString());
                }
            }
        }
        return value;
    }

    private void SetDictionaryValuesFromExpression(Dictionary<string, object> dictionary, Expression expression, string prefix = "")
    {
        switch (expression)
        {
            case MemberInitExpression memberInitExpression:

                foreach (var binding in memberInitExpression.Bindings)
                {
                    var memberAssignment = binding as MemberAssignment;
                    var propName = binding.Member.Name;

                    switch (memberAssignment.Expression)
                    {
                        case MemberInitExpression exp:
                            SetDictionaryValuesFromExpression(dictionary, exp, $"{prefix}{propName}.");
                            break;

                        case ConstantExpression exp:
                            dictionary[$"{prefix}{propName}"] = exp.Value;
                            break;
                    }
                }

                break;

            case MemberExpression memberExpression:
                var constantExpression = memberExpression.Expression as ConstantExpression;
                var memberValue = constantExpression.Value;
                var value = memberValue.GetType().GetField(memberExpression.Member.Name).GetValue(memberValue);
                var dict = GetDictionaryFromObject(value, prefix);

                foreach (var kvp in dict)
                {
                    dictionary[kvp.Key] = kvp.Value;
                }

                break;

            default:

                throw new NotSupportedException();
        }
    }
    
    private static TConfig GetConfig<TConfig>(Action<TConfig> configuration)
        where TConfig : Configuration<TEntity>
    {
        Configuration<TEntity> config;
        if (configuration is Action<QueryableConfiguration<TEntity>>)
        {
            config = new QueryableConfiguration<TEntity>();
        }
        else
        {
            config = new DefaultConfiguration<TEntity>();
        }

        configuration.Invoke((TConfig)config);

        return (TConfig)config;
    }

    internal IMongoQueryable<TEntity> GetQueryable<TConfig>(Action<TConfig> configuration,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> internalQueryAction)
        where TConfig : Configuration<TEntity>
    {
        if (configuration == null)
        {
            return (IMongoQueryable<TEntity>)internalQueryAction.Invoke(this);
        }

        var config = GetConfig(configuration);
        var queryableConfig = config as QueryableConfiguration<TEntity>;

        IMongoQueryable<TEntity> query = this;

        if (config.Properties?.Any() == true)
        {
            query = (IMongoQueryable<TEntity>)query.IncludeMany(_database, config.Properties.ToArray());
        }

        if (queryableConfig != null)
        {
            query = (IMongoQueryable<TEntity>)queryableConfig.BeforeCustomization.Invoke(query);
        }

        query = (IMongoQueryable<TEntity>)internalQueryAction.Invoke(query);

        if (config.SortingColumns.Any())
        {
            query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(config.SortingColumns.ToArray());
        }

        if (queryableConfig != null)
        {
            query = (IMongoQueryable<TEntity>)queryableConfig.AfterCustomization.Invoke(query);
        }
        
        return query;
    }
}