using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Config;
using eQuantic.Core.Data.Repository.Read;
using eQuantic.Linq.Extensions;
using eQuantic.Linq.Specification;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace eQuantic.Core.Data.MongoDb.Repository.Read;

public class AsyncQueryableReadRepository<TUnitOfWork, TEntity, TKey> : 
    QueryableReadRepository<TUnitOfWork, TEntity, TKey>,
    IAsyncQueryableReadRepository<TUnitOfWork, TEntity, TKey>
    where TUnitOfWork : IQueryableUnitOfWork
    where TEntity : class, IEntity, new()
{
    /// <summary>
    /// Create a new instance of repository
    /// </summary>
    /// <param name="unitOfWork">Associated Unit Of Work</param>
    public AsyncQueryableReadRepository(TUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    public Task<IEnumerable<TEntity>> AllMatchingAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return AllMatchingAsync(specification, configuration, CancellationToken.None);
    }
    
    public async Task<IEnumerable<TEntity>> AllMatchingAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        IMongoQueryable<TEntity> query = GetSet().Where(specification.SatisfiedBy());
        return await query.ToListAsync(cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> AllMatchingAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken)
    {
        return AllMatchingAsync(specification, _ => { }, cancellationToken);
    }

    public Task<long> CountAsync(CancellationToken cancellationToken = new())
    {
        return GetSet().LongCountAsync(cancellationToken);
    }

    public Task<long> CountAsync(
        ISpecification<TEntity> specification, 
        CancellationToken cancellationToken = new())
    {
        return CountAsync(specification.SatisfiedBy(), cancellationToken);
    }

    public Task<long> CountAsync(
        Expression<Func<TEntity, bool>> filter, 
        CancellationToken cancellationToken = new())
    {
        return GetSet().LongCountAsync(filter, cancellationToken);
    }

    public Task<bool> AllAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return AllAsync(specification, configuration, CancellationToken.None);
    }

    public async Task<bool> AllAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return !(await GetSet().AnyAsync(specification.SatisfiedBy(), cancellationToken));
    }
    
    public Task<bool> AllAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken)
    {
        return AllAsync(specification, _ => { }, cancellationToken);
    }
    
    public Task<bool> AllAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return AllAsync(filter, configuration, CancellationToken.None);
    }

    public async Task<bool> AllAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return !(await GetSet().AnyAsync(filter, cancellationToken));
    }
    
    public Task<bool> AllAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken)
    {
        return AllAsync(filter, _ => { }, cancellationToken);
    }
    
    public Task<bool> AnyAsync(
        Action<QueryableConfiguration<TEntity>> configuration = null, 
        CancellationToken cancellationToken = new())
    {
        return GetSet().AnyAsync(cancellationToken);
    }

    public Task<bool> AnyAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return AnyAsync(specification, configuration, CancellationToken.None);
    }
    
    public Task<bool> AnyAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return GetSet().AnyAsync(specification.SatisfiedBy(), cancellationToken);
    }
    
    public Task<bool> AnyAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken)
    {
        return AnyAsync(specification, _ => { }, cancellationToken);
    }

    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return AnyAsync(filter, configuration, CancellationToken.None);
    }
    
    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return GetSet().AnyAsync(filter, cancellationToken);
    }
    
    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken)
    {
        return AnyAsync(filter, _ => { }, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> GetAllAsync(Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetAllAsync(configuration, CancellationToken.None);
    }
    
    public async Task<IEnumerable<TEntity>> GetAllAsync(
        Action<QueryableConfiguration<TEntity>> configuration, 
        CancellationToken cancellationToken)
    {
        var query = GetSet().GetQueryable(configuration, o => o);
        return await query.ToListAsync(cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return GetAllAsync(_ => { }, cancellationToken);
    }

    public Task<TEntity> GetAsync(
        TKey id, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetAsync(id, configuration, CancellationToken.None);
    }
    
    public Task<TEntity> GetAsync(
        TKey id, 
        Action<QueryableConfiguration<TEntity>> configuration, 
        CancellationToken cancellationToken)
    {
        return GetSet().FindAsync(id, cancellationToken);
    }
    
    public Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken)
    {
        return GetAsync(id, _ => { }, cancellationToken);
    }

    public Task<IEnumerable<TResult>> GetMappedAsync<TResult>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetMappedAsync(filter, map, configuration, CancellationToken.None);
    }
    
    public Task<IEnumerable<TResult>> GetMappedAsync<TResult>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    public Task<IEnumerable<TResult>> GetMappedAsync<TResult>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TResult>> map,
        CancellationToken cancellationToken)
    {
        return GetMappedAsync(filter, map, _ => { }, cancellationToken);
    }

    public Task<IEnumerable<TResult>> GetMappedAsync<TResult>(
        ISpecification<TEntity> specification, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetMappedAsync(specification, map, configuration, CancellationToken.None);
    }
    
    public Task<IEnumerable<TResult>> GetMappedAsync<TResult>(
        ISpecification<TEntity> specification, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    public Task<IEnumerable<TResult>> GetMappedAsync<TResult>(
        ISpecification<TEntity> specification, 
        Expression<Func<TEntity, TResult>> map,
        CancellationToken cancellationToken)
    {
        return GetMappedAsync(specification, map, _ => { }, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> GetFilteredAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetFilteredAsync(filter, configuration, CancellationToken.None);
    }

    public async Task<IEnumerable<TEntity>> GetFilteredAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        if (filter == null)
            throw new ArgumentException("Filter expression cannot be null", nameof(filter));

        var query = GetSet()
            .GetQueryable(configuration, o => o.Where(filter));

        return await query.ToListAsync(cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetFilteredAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken)
    {
        return GetFilteredAsync(filter, _ => { }, cancellationToken);
    }
    
    public Task<TEntity> GetFirstAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetFirstAsync(filter, configuration, CancellationToken.None);
    }

    public Task<TEntity> GetFirstAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        var query = GetSet()
            .GetQueryable(configuration, filter != null ? o => o.Where(filter) : o => o);
        
        return query.FirstOrDefaultAsync(cancellationToken);
    }
    
    public Task<TEntity> GetFirstAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken)
    {
        return GetFirstAsync(filter, _ => { }, cancellationToken);
    }
    
    public Task<TEntity> GetFirstAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetFirstAsync(specification, configuration, CancellationToken.None);
    }
    
    public Task<TEntity> GetFirstAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return GetFirstAsync(specification?.SatisfiedBy(), configuration, cancellationToken);
    }
    
    public Task<TEntity> GetFirstAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken)
    {
        return GetFirstAsync(specification, _ => {}, cancellationToken);
    }

    public Task<TResult> GetFirstMappedAsync<TResult>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetFirstMappedAsync(filter, map, configuration, CancellationToken.None);
    }
    
    public Task<TResult> GetFirstMappedAsync<TResult>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    public Task<TResult> GetFirstMappedAsync<TResult>(
        Expression<Func<TEntity, bool>> filter, 
        Expression<Func<TEntity, TResult>> map,
        CancellationToken cancellationToken)
    {
        return GetFirstMappedAsync(filter, map, _ => { }, cancellationToken);
    }

    public Task<TResult> GetFirstMappedAsync<TResult>(
        ISpecification<TEntity> specification, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetFirstMappedAsync(specification, map, configuration, CancellationToken.None);
    }
    
    public Task<TResult> GetFirstMappedAsync<TResult>(
        ISpecification<TEntity> specification, 
        Expression<Func<TEntity, TResult>> map, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    public Task<TResult> GetFirstMappedAsync<TResult>(
        ISpecification<TEntity> specification, 
        Expression<Func<TEntity, TResult>> map,
        CancellationToken cancellationToken)
    {
        return GetFirstMappedAsync(specification, map, _ => { }, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(
        int limit, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetPagedAsync(limit, configuration, CancellationToken.None);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(
        int limit, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return GetPagedAsync((Expression<Func<TEntity, bool>>)null, 1, limit, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        int limit,
        CancellationToken cancellationToken)
    {
        return GetPagedAsync(limit, _ => {}, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification, 
        int limit, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetPagedAsync(specification, limit, configuration, CancellationToken.None);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification, 
        int limit, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return GetPagedAsync(specification.SatisfiedBy(), 1, limit, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification, 
        int limit,
        CancellationToken cancellationToken)
    {
        return GetPagedAsync(specification, limit, _ => {}, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>> filter, 
        int limit, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetPagedAsync(filter, limit, configuration, CancellationToken.None);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>> filter, 
        int limit, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return GetPagedAsync(filter, 1, limit, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>> filter, 
        int limit,
        CancellationToken cancellationToken)
    {
        return GetPagedAsync(filter, limit, _ => {}, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        int pageIndex, 
        int pageSize, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetPagedAsync(pageIndex, pageSize, configuration, CancellationToken.None);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        int pageIndex, 
        int pageSize, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return GetPagedAsync((Expression<Func<TEntity, bool>>)null, pageIndex, pageSize, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        int pageIndex, 
        int pageSize,
        CancellationToken cancellationToken)
    {
        return GetPagedAsync(pageIndex, pageSize, _ => {}, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification, 
        int pageIndex, 
        int pageSize, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetPagedAsync(specification, pageIndex, pageSize, configuration, CancellationToken.None);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification, 
        int pageIndex, 
        int pageSize, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        return GetPagedAsync(specification.SatisfiedBy(), pageIndex, pageSize, configuration, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification, 
        int pageIndex, 
        int pageSize,
        CancellationToken cancellationToken)
    {
        return GetPagedAsync(specification, pageIndex, pageSize, _ => {}, cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>> filter, 
        int pageIndex, 
        int pageSize, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetPagedAsync(filter, pageIndex, pageSize, configuration, CancellationToken.None);
    }

    public async Task<IEnumerable<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>> filter, 
        int pageIndex, 
        int pageSize, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        var query = GetSet()
            .GetQueryable(configuration, filter != null ? o => o.Where(filter) : o => o);
        if (pageSize > 0)
        {
            int skip = (pageIndex - 1) * pageSize;
            return await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);
        }

        return await query.ToListAsync(cancellationToken);
    }
    
    public Task<IEnumerable<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>> filter, 
        int pageIndex, 
        int pageSize,
        CancellationToken cancellationToken = new())
    {
        return GetPagedAsync(filter, pageIndex, pageSize, _ => {}, cancellationToken);
    }

    public Task<TEntity> GetSingleAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetSingleAsync(filter, configuration, CancellationToken.None);
    }
    
    public Task<TEntity> GetSingleAsync(
        Expression<Func<TEntity, bool>> filter, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        if (filter == null)
            throw new ArgumentException("Filter expression cannot be null", nameof(filter));
        
        var query = GetSet().GetQueryable(configuration, o => o.Where(filter));
        return query.SingleOrDefaultAsync(cancellationToken);
    }
    
    public Task<TEntity> GetSingleAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken)
    {
        return GetSingleAsync(filter, _ => {}, cancellationToken);
    }

    public Task<TEntity> GetSingleAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetSingleAsync(specification, configuration, CancellationToken.None);
    }
    
    public Task<TEntity> GetSingleAsync(
        ISpecification<TEntity> specification, 
        Action<QueryableConfiguration<TEntity>> configuration,
        CancellationToken cancellationToken)
    {
        if (specification == null)
            throw new ArgumentException("The specification cannot be null", nameof(specification));
        return GetSingleAsync(specification.SatisfiedBy(), configuration, cancellationToken);
    }
    
    public Task<TEntity> GetSingleAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = new())
    {
        return GetSingleAsync(specification, _ => {}, cancellationToken);
    }
}