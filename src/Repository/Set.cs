using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using eQuantic.Core.Data.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eQuantic.Core.Data.MongoDb.Repository
{
    public class Set<TEntity> : Data.Repository.ISet<TEntity>, IMongoQueryable<TEntity>
        where TEntity : class, IEntity, new()
    {
        private const string IdKey = "id";
        private readonly IMongoCollection<TEntity> collection;
        private readonly IMongoQueryable<TEntity> internalQueryable;

        public Set(IMongoDatabase database)
        {
            this.collection = database.GetCollection<TEntity>(typeof(TEntity).Name);
            this.internalQueryable = collection.AsQueryable();
        }

        public Type ElementType => internalQueryable.ElementType;

        public Expression Expression => internalQueryable.Expression;

        public IQueryProvider Provider => internalQueryable.Provider;

        public void Delete(Expression<Func<TEntity, bool>> filter)
        {
            this.collection.DeleteOne(filter);
        }

        public Task DeleteAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        {
            return this.collection.DeleteOneAsync(filter, cancellationToken);
        }

        public long DeleteMany(Expression<Func<TEntity, bool>> filter)
        {
            var result = this.collection.DeleteMany(filter);
            return result.DeletedCount;
        }

        public async Task<long> DeleteManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        {
            var result = await this.collection.DeleteManyAsync(filter, cancellationToken);
            return result.DeletedCount;
        }

        public IEnumerable<TEntity> Execute()
        {
            return internalQueryable.ToList();
        }

        public IFindFluent<TEntity, TEntity> Find(Expression<Func<TEntity, bool>> filter)
        {
            return this.collection.Find(filter);
        }

        public TEntity Find<TKey>(TKey key)
        {
            if (Convert.GetTypeCode(key) == TypeCode.Object)
            {
                var expression = GetKeyExpression(key);
                return this.collection.Find(expression).FirstOrDefault();
            }
            var filter = Builders<TEntity>.Filter.Eq(IdKey, ObjectId.Parse(key.ToString()));
            return this.collection.Find(filter).FirstOrDefault();
        }

        public async Task<TEntity> FindAsync<TKey>(TKey key, CancellationToken cancellationToken = default)
        {
            IAsyncCursor<TEntity> cursor;
            if (Convert.GetTypeCode(key) == TypeCode.Object)
            {
                var expression = GetKeyExpression(key);
                cursor = await this.collection.FindAsync(expression, null, cancellationToken);
                return cursor.FirstOrDefault();
            }
            var filter = Builders<TEntity>.Filter.Eq(IdKey, ObjectId.Parse(key.ToString()));
            cursor = await this.collection.FindAsync(filter, null, cancellationToken);
            return cursor.FirstOrDefault();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return internalQueryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalQueryable.GetEnumerator();
        }

        public QueryableExecutionModel GetExecutionModel()
        {
            return internalQueryable.GetExecutionModel();
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
            this.collection.InsertOne(item);
        }

        public Task InsertAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            return this.collection.InsertOneAsync(item, null, cancellationToken);
        }

        public IAsyncCursor<TEntity> ToCursor(CancellationToken cancellationToken = default)
        {
            return internalQueryable.ToCursor(cancellationToken);
        }

        public Task<IAsyncCursor<TEntity>> ToCursorAsync(CancellationToken cancellationToken = default)
        {
            return internalQueryable.ToCursorAsync(cancellationToken);
        }

        public long Update(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity>> updateExpression)
        {
            var updateDefinition = GetUpdateDefinition(updateExpression);
            var result = this.collection.UpdateOne<TEntity>(filter, updateDefinition);
            return result.ModifiedCount;
        }

        public async Task<long> UpdateAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity>> updateExpression, CancellationToken cancellationToken = default)
        {
            var updateDefinition = GetUpdateDefinition(updateExpression);
            var result = await this.collection.UpdateOneAsync<TEntity>(filter, updateDefinition, null, cancellationToken);
            return result.ModifiedCount;
        }

        public long UpdateMany(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity>> updateExpression)
        {
            var updateDefinition = GetUpdateDefinition(updateExpression);
            var result = this.collection.UpdateMany(filter, updateDefinition);
            return result.ModifiedCount;
        }

        public async Task<long> UpdateManyAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity>> updateExpression, CancellationToken cancellationToken = default)
        {
            var updateDefinition = GetUpdateDefinition(updateExpression);
            var result = await this.collection.UpdateManyAsync(filter, updateDefinition, null, cancellationToken);
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
            return new[] { IdKey };
        }

        private UpdateDefinition<TEntity> GetUpdateDefinition(Expression<Func<TEntity>> updateExpression)
        {
            var values = GetDictionaryFromExpression(updateExpression.Body);

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
    }
}