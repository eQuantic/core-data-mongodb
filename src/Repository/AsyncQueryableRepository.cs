using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using eQuantic.Core.Data.MongoDb.Repository.Read;
using eQuantic.Core.Data.MongoDb.Repository.Write;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Config;
using eQuantic.Core.Data.Repository.Read;
using eQuantic.Core.Data.Repository.Write;
using eQuantic.Linq.Specification;

namespace eQuantic.Core.Data.MongoDb.Repository;

public class AsyncQueryableRepository<TUnitOfWork, TEntity, TKey> : 
    QueryableRepository<TUnitOfWork, TEntity, TKey>,
    IAsyncQueryableRepository<TUnitOfWork, TEntity, TKey>
    where TUnitOfWork : IQueryableUnitOfWork
    where TEntity : class, IEntity, new()
{
    private readonly IAsyncQueryableReadRepository<TUnitOfWork, TEntity, TKey> _readRepository;
    private readonly IAsyncWriteRepository<TUnitOfWork, TEntity> _writeRepository;

    public AsyncQueryableRepository(TUnitOfWork unitOfWork) : base(unitOfWork)
    {
        _readRepository = new AsyncQueryableReadRepository<TUnitOfWork, TEntity, TKey>(UnitOfWork);
        _writeRepository = new AsyncWriteRepository<TUnitOfWork, TEntity, TKey>(UnitOfWork);
    }

    public Task AddAsync(TEntity item)
    {
        return _writeRepository.AddAsync(item);
    }

    public Task<long> DeleteManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = new ())
    {
        return _writeRepository.DeleteManyAsync(filter, cancellationToken);
    }

    public Task<long> DeleteManyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = new CancellationToken())
    {
        return _writeRepository.DeleteManyAsync(specification, cancellationToken);
    }

    public Task MergeAsync(TEntity persisted, TEntity current)
    {
        return _writeRepository.MergeAsync(persisted, current);
    }

    public async Task ModifyAsync(TEntity item)
    {
        await _writeRepository.ModifyAsync(item);
    }

    public async Task RemoveAsync(TEntity item)
    {
        await _writeRepository.RemoveAsync(item);
    }

    public Task<long> UpdateManyAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> updateFactory,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return _writeRepository.UpdateManyAsync(filter, updateFactory, cancellationToken);
    }

    public Task<long> UpdateManyAsync(ISpecification<TEntity> specification, Expression<Func<TEntity, TEntity>> updateFactory,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return _writeRepository.UpdateManyAsync(specification, updateFactory, cancellationToken);
    }
    

    public Task<IEnumerable<TEntity>> AllMatchingAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.AllMatchingAsync(specification, configuration);
    }
    
    public Task<IEnumerable<TEntity>> AllMatchingAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.AllMatchingAsync(specification, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> AllMatchingAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken)
    {
        return _readRepository.AllMatchingAsync(specification, cancellationToken);
    }

    public Task<long> CountAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return _readRepository.CountAsync(cancellationToken);
    }

    public Task<long> CountAsync(
        ISpecification<TEntity> specification, 
        CancellationToken cancellationToken = new CancellationToken())
    {
        return _readRepository.CountAsync(specification, cancellationToken);
    }

    public Task<long> CountAsync(
        Expression<Func<TEntity, bool>> filter, 
        CancellationToken cancellationToken = new CancellationToken())
    {
        return _readRepository.CountAsync(filter, cancellationToken);
    }

    public Task<bool> AllAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.AllAsync(specification, configuration);
    }
    
    public Task<bool> AllAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.AllAsync(specification, configuration, cancellationToken);
    }
    
    public Task<bool> AllAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken)
    {
        return _readRepository.AllAsync(specification, cancellationToken);
    }

    public Task<bool> AllAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.AllAsync(filter, configuration);
    }
    
    public Task<bool> AllAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.AllAsync(filter, configuration, cancellationToken);
    }
    
    public Task<bool> AllAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken)
    {
        return _readRepository.AllAsync(filter, cancellationToken);
    }

    public Task<bool> AnyAsync(
        Action<QueryableConfiguration<TEntity>> configuration = null, 
        CancellationToken cancellationToken = new CancellationToken())
    {
        return _readRepository.AnyAsync(configuration, cancellationToken);
    }

    public Task<bool> AnyAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.AnyAsync(specification, configuration);
    }
    
    public Task<bool> AnyAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.AnyAsync(specification, configuration, cancellationToken);
    }
    
    public Task<bool> AnyAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken)
    {
        return _readRepository.AnyAsync(specification, cancellationToken);
    }

    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.AnyAsync(filter, configuration);
    }
    
    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.AnyAsync(filter, configuration, cancellationToken);
    }
    
    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken)
    {
        return _readRepository.AnyAsync(filter, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> GetAllAsync(Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetAllAsync(configuration);
    }
    
    public Task<IEnumerable<TEntity>> GetAllAsync(
        Action<QueryableConfiguration<TEntity>> configuration, 
        CancellationToken cancellationToken)
    {
        return _readRepository.GetAllAsync(configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _readRepository.GetAllAsync(cancellationToken);
    }

    public Task<TEntity> GetAsync(
        TKey id, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetAsync(id, configuration);
    }

    public Task<TEntity> GetAsync(
        TKey id, 
        Action<QueryableConfiguration<TEntity>> configuration, 
        CancellationToken cancellationToken)
    {
        return _readRepository.GetAsync(id, configuration, cancellationToken);
    }
    
    public Task<TEntity> GetAsync(
        TKey id,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetAsync(id, cancellationToken);
    }
    
    public Task<IEnumerable<TResult>> GetMappedAsync<TResult>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetMappedAsync(filter, map, configuration);
    }

    public Task<IEnumerable<TResult>> GetMappedAsync<TResult>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetMappedAsync(filter, map, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TResult>> GetMappedAsync<TResult>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TResult>> map,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetMappedAsync(filter, map, cancellationToken);
    }
    
    public Task<IEnumerable<TResult>> GetMappedAsync<TResult>(
        ISpecification<TEntity> specification, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetMappedAsync(specification, map, configuration);
    }
    
    public Task<IEnumerable<TResult>> GetMappedAsync<TResult>(
        ISpecification<TEntity> specification, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetMappedAsync(specification, map, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TResult>> GetMappedAsync<TResult>(
        ISpecification<TEntity> specification, 
        Expression<Func<TEntity, TResult>> map,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetMappedAsync(specification, map, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> GetFilteredAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetFilteredAsync(filter, configuration);
    }

    public Task<IEnumerable<TEntity>> GetFilteredAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetFilteredAsync(filter, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetFilteredAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetFilteredAsync(filter, cancellationToken);
    }
    
    public Task<TEntity> GetFirstAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetFirstAsync(filter, configuration);
    }
    
    public Task<TEntity> GetFirstAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetFirstAsync(filter, configuration, cancellationToken);
    }
    
    public Task<TEntity> GetFirstAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetFirstAsync(filter, cancellationToken);
    }

    public Task<TEntity> GetFirstAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetFirstAsync(specification, configuration);
    }
    
    public Task<TEntity> GetFirstAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetFirstAsync(specification, configuration, cancellationToken);
    }
    
    public Task<TEntity> GetFirstAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetFirstAsync(specification, cancellationToken);
    }

    public Task<TResult> GetFirstMappedAsync<TResult>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetFirstMappedAsync(filter, map, configuration);
    }

    public Task<TResult> GetFirstMappedAsync<TResult>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetFirstMappedAsync(filter, map, configuration, cancellationToken);
    }
    
    public Task<TResult> GetFirstMappedAsync<TResult>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TResult>> map,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetFirstMappedAsync(filter, map, cancellationToken);
    }
    
    public Task<TResult> GetFirstMappedAsync<TResult>(
        ISpecification<TEntity> specification, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetFirstMappedAsync(specification, map, configuration);
    }
    
    public Task<TResult> GetFirstMappedAsync<TResult>(
        ISpecification<TEntity> specification, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetFirstMappedAsync(specification, map, configuration, cancellationToken);
    }
    
    public Task<TResult> GetFirstMappedAsync<TResult>(
        ISpecification<TEntity> specification, 
        Expression<Func<TEntity, TResult>> map,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetFirstMappedAsync(specification, map, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(
        int limit, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetPagedAsync(limit, configuration);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(
        int limit, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetPagedAsync(limit, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        int limit,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetPagedAsync(limit, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification, 
        int limit, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetPagedAsync(specification, limit, configuration);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification, 
        int limit, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetPagedAsync(specification, limit, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification, 
        int limit,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetPagedAsync(specification, limit, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>> filter, 
        int limit, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetPagedAsync(filter, limit, configuration);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>> filter, 
        int limit, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetPagedAsync(filter, limit, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>> filter, 
        int limit,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetPagedAsync(filter, limit, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        int pageIndex, 
        int pageSize, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetPagedAsync(pageIndex, pageSize, configuration);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(
        int pageIndex, 
        int pageSize, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetPagedAsync(pageIndex, pageSize, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        int pageIndex, 
        int pageSize,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetPagedAsync(pageIndex, pageSize, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification, 
        int pageIndex, 
        int pageSize, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetPagedAsync(specification, pageIndex, pageSize, configuration);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification, 
        int pageIndex, 
        int pageSize, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetPagedAsync(specification, pageIndex, pageSize, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification, 
        int pageIndex, 
        int pageSize,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetPagedAsync(specification, pageIndex, pageSize, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>> filter, 
        int pageIndex, 
        int pageSize, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetPagedAsync(filter, pageIndex, pageSize, configuration);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>> filter, 
        int pageIndex, 
        int pageSize, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetPagedAsync(filter, pageIndex, pageSize, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>> filter, 
        int pageIndex, 
        int pageSize,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetPagedAsync(filter, pageIndex, pageSize, cancellationToken);
    }

    public Task<TEntity> GetSingleAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetSingleAsync(filter, configuration);
    }

    public Task<TEntity> GetSingleAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetSingleAsync(filter, configuration, cancellationToken);
    }
    
    public Task<TEntity> GetSingleAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetSingleAsync(filter, cancellationToken);
    }
    
    public Task<TEntity> GetSingleAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return _readRepository.GetSingleAsync(specification, configuration);
    }
    
    public Task<TEntity> GetSingleAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetSingleAsync(specification, configuration, cancellationToken);
    }
    
    public Task<TEntity> GetSingleAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken)
    {
        return _readRepository.GetSingleAsync(specification, cancellationToken);
    }
}