using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Read;
using eQuantic.Core.Linq.Extensions;
using eQuantic.Core.Linq.Sorter;
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
        /// <see cref="eQuantic.Core.Data.Repository.Read.IAsyncReadRepository{TUnitOfWork, TEntity, TKey}"/>
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

        public async Task<IEnumerable<TEntity>> GetAllAsync(params ISorting[] sortingColumns)
        {
            if (sortingColumns != null && sortingColumns.Length > 0)
            {
                return await ((IOrderedMongoQueryable<TEntity>)GetSet().OrderBy(sortingColumns)).ToListAsync();
            }
            return await GetSet().ToListAsync();
        }

        public Task<TEntity> GetAsync(TKey id)
        {
            return GetSet().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetFilteredAsync(Expression<Func<TEntity, bool>> filter, params ISorting[] sortColumns)
        {
            if (filter == null)
                throw new ArgumentException("Filter expression cannot be null", nameof(filter));

            IMongoQueryable<TEntity> query = GetSet().Where(filter);

            if (sortColumns != null && sortColumns.Length > 0)
            {
                query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(sortColumns);
            }

            return await query.ToListAsync();
        }

        public Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> filter, params ISorting[] sortingColumns)
        {
            IMongoQueryable<TEntity> query = GetSet();
            if (filter != null) query = query.Where(filter);

            if (sortingColumns != null && sortingColumns.Length > 0)
            {
                query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(sortingColumns);
            }

            return query.FirstOrDefaultAsync();
        }

        public Task<TEntity> GetFirstAsync(ISpecification<TEntity> specification, params ISorting[] sortingColumns)
        {
            return GetFirstAsync(specification.SatisfiedBy(), sortingColumns);
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(int limit, params ISorting[] sortColumns)
        {
            return GetPagedAsync((Expression<Func<TEntity, bool>>)null, 1, limit, sortColumns);
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, int limit, params ISorting[] sortColumns)
        {
            return GetPagedAsync(specification.SatisfiedBy(), 1, limit, sortColumns);
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> filter, int limit, params ISorting[] sortColumns)
        {
            return GetPagedAsync(filter, 1, limit, sortColumns);
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(int pageIndex, int pageCount, params ISorting[] sortColumns)
        {
            return GetPagedAsync((Expression<Func<TEntity, bool>>)null, pageIndex, pageCount, sortColumns);
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, int pageIndex, int pageCount, params ISorting[] sortColumns)
        {
            return GetPagedAsync(specification.SatisfiedBy(), pageIndex, pageCount, sortColumns);
        }

        public async Task<IEnumerable<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> filter, int pageIndex, int pageCount, params ISorting[] sortColumns)
        {
            IMongoQueryable<TEntity> query = GetSet();
            if (filter != null) query = query.Where(filter);

            if (sortColumns != null && sortColumns.Length > 0)
            {
                query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(sortColumns);
            }
            if (pageCount > 0)
            {
                int skip = (pageIndex - 1) * pageCount;
                return await query.Skip(skip).Take(pageCount).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter, params ISorting[] sortingColumns)
        {
            if (filter == null)
                throw new ArgumentException("Filter expression cannot be null", nameof(filter));

            IMongoQueryable<TEntity> query = GetSet().Where(filter);

            if (sortingColumns != null && sortingColumns.Length > 0)
            {
                query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(sortingColumns);
            }
            return query.SingleOrDefaultAsync();
        }

        public Task<TEntity> GetSingleAsync(ISpecification<TEntity> specification, params ISorting[] sortingColumns)
        {
            return GetSingleAsync(specification.SatisfiedBy(), sortingColumns);
        }

        protected Set<TEntity> GetSet()
        {
            return _dbSet ??= (Set<TEntity>)UnitOfWork.CreateSet<TEntity>();
        }
    }
}