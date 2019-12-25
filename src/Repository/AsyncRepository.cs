using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eQuantic.Core.Data.MongoDb.Repository.Read;
using eQuantic.Core.Data.MongoDb.Repository.Write;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Read;
using eQuantic.Core.Data.Repository.Write;
using eQuantic.Core.Linq;
using eQuantic.Core.Linq.Specification;

namespace eQuantic.Core.Data.MongoDb.Repository
{
    public class AsyncRepository<TUnitOfWork, TEntity, TKey> : IAsyncRepository<TUnitOfWork, TEntity, TKey>
        where TUnitOfWork : IQueryableUnitOfWork
        where TEntity : class, IEntity, new()
    {
        private readonly IAsyncReadRepository<TUnitOfWork, TEntity, TKey> readRepository;
        private readonly IAsyncWriteRepository<TUnitOfWork, TEntity, TKey> writeRepository;

        public AsyncRepository(TUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            UnitOfWork = unitOfWork;

            readRepository = new AsyncReadRepository<TUnitOfWork, TEntity, TKey>(UnitOfWork);
            writeRepository = new AsyncWriteRepository<TUnitOfWork, TEntity, TKey>(UnitOfWork);
        }

        /// <summary>
        /// <see cref="eQuantic.Core.Data.Repository.IAsyncRepository{TUnitOfWork, TEntity, TKey}"/>
        /// </summary>
        public TUnitOfWork UnitOfWork { get; private set; }

        public Task AddAsync(TEntity item)
        {
            return writeRepository.AddAsync(item);
        }

        public Task<IEnumerable<TEntity>> AllMatchingAsync(ISpecification<TEntity> specification)
        {
            return readRepository.AllMatchingAsync(specification);
        }

        public Task<long> CountAsync()
        {
            return readRepository.CountAsync();
        }

        public Task<long> CountAsync(ISpecification<TEntity> specification)
        {
            return readRepository.CountAsync(specification);
        }

        public Task<long> CountAsync(Expression<Func<TEntity, bool>> filter)
        {
            return readRepository.CountAsync(filter);
        }

        public Task<long> DeleteManyAsync(Expression<Func<TEntity, bool>> filter)
        {
            return writeRepository.DeleteManyAsync(filter);
        }

        public Task<long> DeleteManyAsync(ISpecification<TEntity> specification)
        {
            return writeRepository.DeleteManyAsync(specification);
        }

        public void Dispose()
        {
            UnitOfWork?.Dispose();
        }

        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return readRepository.GetAllAsync();
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(ISorting[] sortingColumns)
        {
            return readRepository.GetAllAsync(sortingColumns);
        }

        public Task<TEntity> GetAsync(TKey id)
        {
            return readRepository.GetAsync(id);
        }

        public Task<IEnumerable<TEntity>> GetFilteredAsync(Expression<Func<TEntity, bool>> filter)
        {
            return readRepository.GetFilteredAsync(filter);
        }

        public Task<IEnumerable<TEntity>> GetFilteredAsync(Expression<Func<TEntity, bool>> filter, ISorting[] sortColumns)
        {
            return readRepository.GetFilteredAsync(filter, sortColumns);
        }

        public Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> filter)
        {
            return readRepository.GetFirstAsync(filter);
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(int limit, ISorting[] sortColumns)
        {
            return readRepository.GetPagedAsync(limit, sortColumns);
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, int limit, ISorting[] sortColumns)
        {
            return readRepository.GetPagedAsync(specification, limit, sortColumns);
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> filter, int limit, ISorting[] sortColumns)
        {
            return readRepository.GetPagedAsync(filter, limit, sortColumns);
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(int pageIndex, int pageCount, ISorting[] sortColumns)
        {
            return readRepository.GetPagedAsync(pageIndex, pageCount, sortColumns);
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, int pageIndex, int pageCount, ISorting[] sortColumns)
        {
            return readRepository.GetPagedAsync(specification, pageIndex, pageCount, sortColumns);
        }

        public Task<IEnumerable<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> filter, int pageIndex, int pageCount, ISorting[] sortColumns)
        {
            return readRepository.GetPagedAsync(filter, pageIndex, pageCount, sortColumns);
        }

        public Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter)
        {
            return readRepository.GetSingleAsync(filter);
        }

        public Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter, ISorting[] sortingColumns)
        {
            return readRepository.GetSingleAsync(filter, sortingColumns);
        }

        public Task MergeAsync(TEntity persisted, TEntity current)
        {
            return writeRepository.MergeAsync(persisted, current);
        }

        public async Task ModifyAsync(TEntity item)
        {
            await writeRepository.ModifyAsync(item);
        }

        public async Task RemoveAsync(TEntity item)
        {
            await writeRepository.RemoveAsync(item);
        }

        public Task<long> UpdateManyAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> updateFactory)
        {
            return writeRepository.UpdateManyAsync(filter, updateFactory);
        }

        public Task<long> UpdateManyAsync(ISpecification<TEntity> specification, Expression<Func<TEntity, TEntity>> updateFactory)
        {
            return writeRepository.UpdateManyAsync(specification, updateFactory);
        }
    }
}