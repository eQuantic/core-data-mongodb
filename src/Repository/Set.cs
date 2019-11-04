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

namespace eQuantic.Core.Data.MongoDb.Repository
{
    public class Set<TEntity> : Data.Repository.ISet<TEntity>, IMongoQueryable<TEntity>
        where TEntity : class, IEntity, new()
    {
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
            var filter = Builders<TEntity>.Filter.Eq("id", ObjectId.Parse(key.ToString()));
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
            var filter = Builders<TEntity>.Filter.Eq("id", ObjectId.Parse(key.ToString()));
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

        public IAsyncCursor<TEntity> ToCursor(CancellationToken cancellationToken = default)
        {
            return internalQueryable.ToCursor(cancellationToken);
        }

        public Task<IAsyncCursor<TEntity>> ToCursorAsync(CancellationToken cancellationToken = default)
        {
            return internalQueryable.ToCursorAsync(cancellationToken);
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
    }
}