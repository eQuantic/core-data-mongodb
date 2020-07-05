using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Write;
using eQuantic.Core.Linq.Specification;

namespace eQuantic.Core.Data.MongoDb.Repository.Write
{
    public class WriteRepository<TUnitOfWork, TEntity, TKey> : IWriteRepository<TUnitOfWork, TEntity, TKey>
        where TUnitOfWork : IQueryableUnitOfWork
        where TEntity : class, IEntity, new()
    {
        private Set<TEntity> _dbSet = null;

        /// <summary>
        /// Create a new instance of write repository
        /// </summary>
        /// <param name="unitOfWork">Associated Unit Of Work</param>
        public WriteRepository(TUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// <see cref="eQuantic.Core.Data.Repository.Read.IWriteRepository{TUnitOfWork, TEntity, TKey}"/>
        /// </summary>
        public TUnitOfWork UnitOfWork { get; private set; }

        public void Add(TEntity item)
        {
            GetSet().Insert(item);
        }

        public long DeleteMany(Expression<Func<TEntity, bool>> filter)
        {
            return GetSet().DeleteMany(filter);
        }

        public long DeleteMany(ISpecification<TEntity> specification)
        {
            return DeleteMany(specification.SatisfiedBy());
        }

        public void Dispose()
        {
            UnitOfWork?.Dispose();
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

        public long UpdateMany(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> updateFactory)
        {
            return GetSet().UpdateMany(filter, updateFactory);
        }

        public long UpdateMany(ISpecification<TEntity> specification, Expression<Func<TEntity, TEntity>> updateFactory)
        {
            return UpdateMany(specification.SatisfiedBy(), updateFactory);
        }

        protected Set<TEntity> GetSet()
        {
            return _dbSet ??= (Set<TEntity>)UnitOfWork.CreateSet<TEntity>();
        }
    }
}