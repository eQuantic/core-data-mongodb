using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Read;
using eQuantic.Core.Linq.Extensions;
using eQuantic.Core.Linq.Sorter;
using eQuantic.Core.Linq.Specification;
using MongoDB.Driver.Linq;

namespace eQuantic.Core.Data.MongoDb.Repository.Read
{
    public class ReadRepository<TUnitOfWork, TEntity, TKey> : IReadRepository<TUnitOfWork, TEntity, TKey>
        where TUnitOfWork : IQueryableUnitOfWork
        where TEntity : class, IEntity, new()
    {
        private Set<TEntity> _dbSet = null;

        /// <summary>
        /// Create a new instance of repository
        /// </summary>
        /// <param name="unitOfWork">Associated Unit Of Work</param>
        public ReadRepository(TUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// <see cref="eQuantic.Core.Data.Repository.Read.IReadRepository{TUnitOfWork, TEntity, TKey}"/>
        /// </summary>
        public TUnitOfWork UnitOfWork { get; private set; }

        public IEnumerable<TEntity> AllMatching(ISpecification<TEntity> specification, params ISorting[] sortingColumns)
        {
            IMongoQueryable<TEntity> query = GetSet().Where(specification.SatisfiedBy());
            if (sortingColumns != null && sortingColumns.Length > 0)
            {
                query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(sortingColumns);
            }

            return query;
        }

        /// <summary>
        /// <see cref="eQuantic.Core.Data.Repository.Read.IReadRepository{TUnitOfWork, TEntity, TKey}"/>
        /// </summary>
        /// <returns></returns>
        public long Count()
        {
            return GetSet().Count();
        }

        /// <summary>
        /// <see cref="eQuantic.Core.Data.Repository.Read.IReadRepository{TUnitOfWork, TEntity, TKey}"/>
        /// </summary>
        /// <param name="specification">
        /// <see cref="eQuantic.Core.Data.Repository.Read.IReadRepository{TUnitOfWork, TEntity, TKey}"/>
        /// </param>
        /// <returns></returns>
        public long Count(ISpecification<TEntity> specification)
        {
            return GetSet().Where(specification.SatisfiedBy()).Count();
        }

        /// <summary>
        /// <see cref="eQuantic.Core.Data.Repository.Read.IReadRepository{TUnitOfWork, TEntity, TKey}"/>
        /// </summary>
        /// <param name="filter">
        /// <see cref="eQuantic.Core.Data.Repository.Read.IReadRepository{TUnitOfWork, TEntity, TKey}"/>
        /// </param>
        /// <returns></returns>
        public long Count(Expression<Func<TEntity, bool>> filter)
        {
            return GetSet().Where(filter).Count();
        }

        /// <summary>
        /// <see cref="M:System.IDisposable.Dispose"/>
        /// </summary>
        public void Dispose()
        {
            UnitOfWork?.Dispose();
        }

        public TEntity Get(TKey id)
        {
            return id != null ? GetSet().Find(id) : null;
        }

        public IEnumerable<TEntity> GetAll(params ISorting[] sortingColumns)
        {
            IMongoQueryable<TEntity> query = GetSet();
            if (sortingColumns != null && sortingColumns.Length > 0)
            {
                query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(sortingColumns);
            }
            return query.ToList();
        }

        public IEnumerable<TEntity> GetFiltered(Expression<Func<TEntity, bool>> filter, params ISorting[] sortColumns)
        {
            if (filter == null)
                throw new ArgumentException("Filter expression cannot be null", nameof(filter));

            IMongoQueryable<TEntity> query = GetSet().Where(filter);
            if (sortColumns != null && sortColumns.Length > 0)
            {
                query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(sortColumns);
            }
            return query;
        }

        public TEntity GetFirst(Expression<Func<TEntity, bool>> filter, params ISorting[] sortColumns)
        {
            IMongoQueryable<TEntity> query = filter == null ? GetSet() : GetSet().Where(filter);
            if (sortColumns != null && sortColumns.Length > 0)
            {
                query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(sortColumns);
            }

            return query.FirstOrDefault();
        }

        public TEntity GetFirst(ISpecification<TEntity> specification, params ISorting[] sortColumns)
        {
            return GetFirst(specification?.SatisfiedBy(), sortColumns);
        }

        public IEnumerable<TEntity> GetPaged(int limit, params ISorting[] sortColumns)
        {
            return GetPaged((Expression<Func<TEntity, bool>>)null, 1, limit, sortColumns);
        }

        public IEnumerable<TEntity> GetPaged(ISpecification<TEntity> specification, int limit, params ISorting[] sortColumns)
        {
            return GetPaged(specification.SatisfiedBy(), 1, limit, sortColumns);
        }

        public IEnumerable<TEntity> GetPaged(Expression<Func<TEntity, bool>> filter, int limit, params ISorting[] sortColumns)
        {
            return GetPaged(filter, 1, limit, sortColumns);
        }

        public IEnumerable<TEntity> GetPaged(int pageIndex, int pageCount, params ISorting[] sortColumns)
        {
            return GetPaged((Expression<Func<TEntity, bool>>)null, pageIndex, pageCount, sortColumns);
        }

        public IEnumerable<TEntity> GetPaged(ISpecification<TEntity> specification, int pageIndex, int pageCount, params ISorting[] sortColumns)
        {
            return GetPaged(specification.SatisfiedBy(), pageIndex, pageCount, sortColumns);
        }

        public IEnumerable<TEntity> GetPaged(Expression<Func<TEntity, bool>> filter, int pageIndex, int pageCount, params ISorting[] sortColumns)
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
                return query.Skip(skip).Take(pageCount);
            }

            return query;
        }

        public TEntity GetSingle(Expression<Func<TEntity, bool>> filter, params ISorting[] sortColumns)
        {
            if (filter == null)
                throw new ArgumentException("Filter expression cannot be null", nameof(filter));

            IMongoQueryable<TEntity> query = GetSet().Where(filter);
            if (sortColumns != null && sortColumns.Length > 0)
            {
                query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(sortColumns);
            }

            return query.SingleOrDefault();
        }

        public TEntity GetSingle(ISpecification<TEntity> specification, params ISorting[] sortColumns)
        {
            if (specification == null)
                throw new ArgumentException("The specification cannot be null", nameof(specification));

            return GetSingle(specification.SatisfiedBy(), sortColumns);
        }

        protected Set<TEntity> GetSet()
        {
            return _dbSet ??= (Set<TEntity>)UnitOfWork.CreateSet<TEntity>();
        }
    }
}