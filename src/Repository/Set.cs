﻿using System;
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
            this.collection = database.GetCollection<TEntity>(nameof(TEntity));
            this.internalQueryable = collection.AsQueryable();
        }

        public Type ElementType => internalQueryable.ElementType;

        public Expression Expression => internalQueryable.Expression;

        public IQueryProvider Provider => internalQueryable.Provider;

        public long DeleteMany(Expression<Func<TEntity, bool>> filter)
        {
            var result = this.collection.DeleteMany(filter);
            return result.DeletedCount;
        }

        public IEnumerable<TEntity> Execute()
        {
            return internalQueryable.ToList();
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

        public async Task<TEntity> FindAsync<TKey>(TKey key)
        {
            IAsyncCursor<TEntity> cursor;
            if (Convert.GetTypeCode(key) == TypeCode.Object)
            {
                var expression = GetKeyExpression(key);
                cursor = await this.collection.FindAsync(expression);
                return cursor.FirstOrDefault();
            }
            var filter = Builders<TEntity>.Filter.Eq(IdKey, ObjectId.Parse(key.ToString()));
            cursor = await this.collection.FindAsync(filter);
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

        public IAsyncCursor<TEntity> ToCursor(CancellationToken cancellationToken = default)
        {
            return internalQueryable.ToCursor(cancellationToken);
        }

        public Task<IAsyncCursor<TEntity>> ToCursorAsync(CancellationToken cancellationToken = default)
        {
            return internalQueryable.ToCursorAsync(cancellationToken);
        }

        public void Update(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            UpdateDefinition<TEntity> updateDefinition = null;

            var updateBuilder = Builders<TEntity>.Update;

            var values = GetDictionaryFromExpression(updateExpression.Body);

            foreach (var kvp in values)
            {
                if (updateDefinition == null)
                    updateDefinition = updateBuilder.Set(kvp.Key, kvp.Value);
                else updateDefinition.Set(kvp.Key, kvp.Value);
            }

            this.collection.UpdateOne(filter, updateDefinition);
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
                                SetDictionaryValuesFromExpression(dictionary, exp, $"{propName}.");
                                break;

                            case ConstantExpression exp:
                                dictionary[$"{prefix}{propName}"] = exp.Value;
                                break;
                        }
                    }

                    break;

                case MemberExpression memberExpression:

                    break;
            }
        }
    }
}