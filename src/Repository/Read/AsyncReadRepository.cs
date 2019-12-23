using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Read;
using eQuantic.Core.Linq;
using eQuantic.Core.Linq.Specification;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace eQuantic.Core.Data.MongoDb.Repository.Read
{
    public class AsyncReadRepository<TUnitOfWork, TEntity, TKey> : IAsyncReadRepository<TUnitOfWork, TEntity, TKey>
        where TUnitOfWork : IQueryableUnitOfWork
        where TEntity : class, IEntity, new()
    {
        private Set<TEntity> _dbSet = null;

        /// <summary>
        /// Create a new instance of repository
        /// </summary>
        /// <param name="unitOfWork">Associated Unit Of Work</param>
        public AsyncReadRepository(TUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// <see cref="eQuantic.Core.Data.Repository.Read.IReadRepository{TUnitOfWork, TEntity, TKey}"/>
        /// </summary>
        public TUnitOfWork UnitOfWork { get; private set; }

        public async Task<IEnumerable<TEntity>> AllMatchingAsync(ISpecification<TEntity> specification)
        {
            return await GetSet().Where(specification.SatisfiedBy()).ToListAsync();
        }

        public Task<long> CountAsync()
        {
            return GetSet().LongCountAsync();
        }

        public Task<long> CountAsync(ISpecification<TEntity> specification)
        {
            return CountAsync(specification.SatisfiedBy());
        }

        public Task<long> CountAsync(Expression<Func<TEntity, bool>> filter)
        {
            return GetSet().LongCountAsync(filter);
        }

        public void Dispose()
        {
            UnitOfWork?.Dispose();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await GetSet().ToListAsync();
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(ISorting[] sortingColumns)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetFilteredAsync(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetFilteredAsync(Expression<Func<TEntity, bool>> filter, ISorting[] sortColumns)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(int limit, ISorting[] sortColumns)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, int limit, ISorting[] sortColumns)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> filter, int limit, ISorting[] sortColumns)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(int pageIndex, int pageCount, ISorting[] sortColumns)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, int pageIndex, int pageCount, ISorting[] sortColumns)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> filter, int pageIndex, int pageCount, ISorting[] sortColumns)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter, ISorting[] sortingColumns)
        {
            throw new NotImplementedException();
        }

        protected Set<TEntity> GetSet()
        {
            return _dbSet ?? (_dbSet = (Set<TEntity>)UnitOfWork.CreateSet<TEntity>());
        }
    }
}