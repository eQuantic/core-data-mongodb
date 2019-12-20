using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using eQuantic.Core.Data.MongoDb.Repository.Read;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Linq.Specification;

namespace eQuantic.Core.Data.MongoDb.Repository
{
    public class Repository<TUnitOfWork, TEntity, TKey> : ReadRepository<TUnitOfWork, TEntity, TKey>, IRepository<TUnitOfWork, TEntity, TKey>
        where TUnitOfWork : IQueryableUnitOfWork
        where TEntity : class, IEntity, new()
    {
        public Repository(TUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Add(TEntity item)
        {
            GetSet().Insert(item);
        }

        public int DeleteMany(Expression<Func<TEntity, bool>> filter)
        {
            return Convert.ToInt32(GetSet().DeleteMany(filter));
        }

        public int DeleteMany(ISpecification<TEntity> specification)
        {
            return DeleteMany(specification.SatisfiedBy());
        }

        public void Merge(TEntity persisted, TEntity current)
        {
            var entityType = typeof(TEntity);

            var properties = entityType.GetTypeInfo().GetProperties()
                .Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(current, null);
                if (value != null)
                    prop.SetValue(persisted, value, null);
            }

            var key = GetSet().GetKeyValue<TKey>(persisted);
            var expression = GetSet().GetKeyExpression(key);

            GetSet().Update(expression, () => persisted);
        }

        public void Modify(TEntity item)
        {
            if (item == (TEntity)null) return;

            var key = GetSet().GetKeyValue<TKey>(item);
            var expression = GetSet().GetKeyExpression(key);

            GetSet().Update(expression, () => item);
        }

        public void Remove(TEntity item)
        {
            var key = GetSet().GetKeyValue<TKey>(item);
            var expression = GetSet().GetKeyExpression(key);

            GetSet().Delete(expression);
        }

        public void TrackItem(TEntity item)
        {
            if (item == (TEntity)null) return;
        }

        public int UpdateMany(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> updateFactory)
        {
            var item = updateFactory.Compile()(null);
            var result = GetSet().UpdateMany(filter, () => item);
            return Convert.ToInt32(result);
        }

        public int UpdateMany(ISpecification<TEntity> specification, Expression<Func<TEntity, TEntity>> updateFactory)
        {
            return UpdateMany(specification.SatisfiedBy(), updateFactory);
        }
    }
}