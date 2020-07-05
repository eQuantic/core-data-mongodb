using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Write;
using eQuantic.Core.Linq.Specification;

namespace eQuantic.Core.Data.MongoDb.Repository.Write
{
    public class AsyncWriteRepository<TUnitOfWork, TEntity, TKey> : IAsyncWriteRepository<TUnitOfWork, TEntity, TKey>
        where TUnitOfWork : IQueryableUnitOfWork
        where TEntity : class, IEntity, new()
    {
        private Set<TEntity> _dbSet = null;

        /// <summary>
        /// Create a new instance of async write repository
        /// </summary>
        /// <param name="unitOfWork">Associated Unit Of Work</param>
        public AsyncWriteRepository(TUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// <see cref="eQuantic.Core.Data.Repository.Read.IAsyncWriteRepository{TUnitOfWork,
        /// TEntity, TKey}"/>
        /// </summary>
        public TUnitOfWork UnitOfWork { get; private set; }

        public Task AddAsync(TEntity item)
        {
            return GetSet().InsertAsync(item);
        }

        public Task<long> DeleteManyAsync(Expression<Func<TEntity, bool>> filter)
        {
            return GetSet().DeleteManyAsync(filter);
        }

        public Task<long> DeleteManyAsync(ISpecification<TEntity> specification)
        {
            return GetSet().DeleteManyAsync(specification.SatisfiedBy());
        }

        public void Dispose()
        {
            UnitOfWork?.Dispose();
        }

        public Task MergeAsync(TEntity persisted, TEntity current)
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

            return GetSet().UpdateAsync(expression, () => persisted);
        }

        public async Task ModifyAsync(TEntity item)
        {
            if (item == (TEntity)null) return;

            var key = GetSet().GetKeyValue<TKey>(item);
            var expression = GetSet().GetKeyExpression(key);

            await GetSet().UpdateAsync(expression, () => item);
        }

        public async Task RemoveAsync(TEntity item)
        {
            if (item == (TEntity)null) return;

            var key = GetSet().GetKeyValue<TKey>(item);
            var expression = GetSet().GetKeyExpression(key);

            await GetSet().DeleteAsync(expression);
        }

        public Task<long> UpdateManyAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> updateFactory)
        {
            return GetSet().UpdateManyAsync(filter, updateFactory);
        }

        public Task<long> UpdateManyAsync(ISpecification<TEntity> specification, Expression<Func<TEntity, TEntity>> updateFactory)
        {
            return UpdateManyAsync(specification.SatisfiedBy(), updateFactory);
        }

        protected Set<TEntity> GetSet()
        {
            return _dbSet ??= (Set<TEntity>)UnitOfWork.CreateSet<TEntity>();
        }
    }
}