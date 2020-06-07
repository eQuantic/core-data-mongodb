using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using eQuantic.Core.Data.MongoDb.Repository.Read;
using eQuantic.Core.Data.MongoDb.Repository.Write;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Read;
using eQuantic.Core.Data.Repository.Write;
using eQuantic.Core.Linq;
using eQuantic.Core.Linq.Sorter;
using eQuantic.Core.Linq.Specification;

namespace eQuantic.Core.Data.MongoDb.Repository
{
    public class Repository<TUnitOfWork, TEntity, TKey> : IRepository<TUnitOfWork, TEntity, TKey>
        where TUnitOfWork : IQueryableUnitOfWork
        where TEntity : class, IEntity, new()
    {
        private readonly IReadRepository<TUnitOfWork, TEntity, TKey> readRepository;
        private readonly IWriteRepository<TUnitOfWork, TEntity, TKey> writeRepository;

        public Repository(TUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            UnitOfWork = unitOfWork;

            readRepository = new ReadRepository<TUnitOfWork, TEntity, TKey>(UnitOfWork);
            writeRepository = new WriteRepository<TUnitOfWork, TEntity, TKey>(UnitOfWork);
        }

        /// <summary>
        /// <see cref="eQuantic.Core.Data.Repository.IRepository{TUnitOfWork, TEntity, TKey}"/>
        /// </summary>
        public TUnitOfWork UnitOfWork { get; private set; }

        public void Add(TEntity item)
        {
            writeRepository.Add(item);
        }
        public IEnumerable<TEntity> AllMatching(ISpecification<TEntity> specification, params ISorting[] sortingColumns)
        {
            return readRepository.AllMatching(specification, sortingColumns);
        }

        public long Count()
        {
            return readRepository.Count();
        }

        public long Count(ISpecification<TEntity> specification)
        {
            return readRepository.Count(specification);
        }

        public long Count(Expression<Func<TEntity, bool>> filter)
        {
            return readRepository.Count(filter);
        }

        public long DeleteMany(Expression<Func<TEntity, bool>> filter)
        {
            return writeRepository.DeleteMany(filter);
        }

        public long DeleteMany(ISpecification<TEntity> specification)
        {
            return writeRepository.DeleteMany(specification);
        }

        public void Dispose()
        {
            UnitOfWork?.Dispose();
        }

        public TEntity Get(TKey id)
        {
            return readRepository.Get(id);
        }

        public IEnumerable<TEntity> GetAll(params ISorting[] sortingColumns)
        {
            return readRepository.GetAll(sortingColumns);
        }

        public IEnumerable<TEntity> GetFiltered(Expression<Func<TEntity, bool>> filter, params ISorting[] sortColumns)
        {
            return readRepository.GetFiltered(filter, sortColumns);
        }

        public TEntity GetFirst(Expression<Func<TEntity, bool>> filter, params ISorting[] sortColumns)
        {
            return readRepository.GetFirst(filter, sortColumns);
        }
        
        public TEntity GetFirst(ISpecification<TEntity> specification, params ISorting[] sortColumns)
        {
            return readRepository.GetFirst(specification, sortColumns);
        }

        public IEnumerable<TEntity> GetPaged(int limit, params ISorting[] sortColumns)
        {
            return readRepository.GetPaged(limit, sortColumns);
        }

        public IEnumerable<TEntity> GetPaged(ISpecification<TEntity> specification, int limit, params ISorting[] sortColumns)
        {
            return readRepository.GetPaged(specification, limit, sortColumns);
        }

        public IEnumerable<TEntity> GetPaged(Expression<Func<TEntity, bool>> filter, int limit, params ISorting[] sortColumns)
        {
            return readRepository.GetPaged(filter, limit, sortColumns);
        }

        public IEnumerable<TEntity> GetPaged(int pageIndex, int pageCount, params ISorting[] sortColumns)
        {
            return readRepository.GetPaged(pageIndex, pageCount, sortColumns);
        }

        public IEnumerable<TEntity> GetPaged(ISpecification<TEntity> specification, int pageIndex, int pageCount, params ISorting[] sortColumns)
        {
            return readRepository.GetPaged(specification, pageIndex, pageCount, sortColumns);
        }

        public IEnumerable<TEntity> GetPaged(Expression<Func<TEntity, bool>> filter, int pageIndex, int pageCount, params ISorting[] sortColumns)
        {
            return readRepository.GetPaged(filter, pageIndex, pageCount, sortColumns);
        }

        public TEntity GetSingle(Expression<Func<TEntity, bool>> filter, params ISorting[] sortColumns)
        {
            return readRepository.GetSingle(filter, sortColumns);
        }

        public TEntity GetSingle(ISpecification<TEntity> specification, params ISorting[] sortColumns)
        {
            return readRepository.GetSingle(specification, sortColumns);
        }
        public void Merge(TEntity persisted, TEntity current)
        {
            writeRepository.Merge(persisted, current);
        }

        public void Modify(TEntity item)
        {
            writeRepository.Modify(item);
        }

        public void Remove(TEntity item)
        {
            writeRepository.Remove(item);
        }

        public void TrackItem(TEntity item)
        {
            writeRepository.TrackItem(item);
        }

        public long UpdateMany(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> updateFactory)
        {
            return writeRepository.UpdateMany(filter, updateFactory);
        }

        public long UpdateMany(ISpecification<TEntity> specification, Expression<Func<TEntity, TEntity>> updateFactory)
        {
            return writeRepository.UpdateMany(specification, updateFactory);
        }
    }
}