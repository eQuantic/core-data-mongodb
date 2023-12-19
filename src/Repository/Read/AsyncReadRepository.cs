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
using eQuantic.Linq.Sorter;
using eQuantic.Linq.Specification;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace eQuantic.Core.Data.MongoDb.Repository.Read;

public class AsyncReadRepository<TUnitOfWork, TEntity, TKey> : IAsyncReadRepository<TUnitOfWork, TEntity, TKey>
    where TUnitOfWork : Configuration<TEntity>, IQueryableUnitOfWork
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

    public void Dispose()
    {
        UnitOfWork?.Dispose();
    }

    protected Set<TEntity> GetSet()
    {
        return _dbSet ??= (Set<TEntity>)UnitOfWork.CreateSet<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> AllMatchingAsync(ISpecification<TEntity> specification, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new())
    {
        IMongoQueryable<TEntity> query = GetSet().Where(specification.SatisfiedBy());

        return await query.ToListAsync(cancellationToken);
    }

    public Task<long> CountAsync(CancellationToken cancellationToken = new())
    {
        return GetSet().LongCountAsync(cancellationToken);
    }

    public Task<long> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = new())
    {
        return CountAsync(specification.SatisfiedBy(), cancellationToken);
    }

    public Task<long> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = new())
    {
        return GetSet().LongCountAsync(filter, cancellationToken);
    }

    public async Task<bool> AllAsync(ISpecification<TEntity> specification, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new())
    {
        return !(await GetSet().AnyAsync(specification.SatisfiedBy(), cancellationToken));
    }

    public async Task<bool> AllAsync(Expression<Func<TEntity, bool>> filter, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new())
    {
        return !(await GetSet().AnyAsync(filter, cancellationToken));
    }

    public Task<bool> AnyAsync(Action<TUnitOfWork> configuration = null, CancellationToken cancellationToken = new())
    {
        return GetSet().AnyAsync(cancellationToken);
    }

    public Task<bool> AnyAsync(ISpecification<TEntity> specification, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new())
    {
        return GetSet().AnyAsync(specification.SatisfiedBy(), cancellationToken);
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new ())
    {
        return GetSet().AnyAsync(filter, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Action<TUnitOfWork> configuration = null, CancellationToken cancellationToken = new CancellationToken())
    {
        var config = Set<TEntity>.GetConfig(configuration);
        if (config.SortingColumns != null && config.SortingColumns.Any())
        {
            return await ((IOrderedMongoQueryable<TEntity>)GetSet().OrderBy(config.SortingColumns.ToArray())).ToListAsync(cancellationToken);
        }
        return await GetSet().ToListAsync(cancellationToken);
    }

    public Task<TEntity> GetAsync(TKey id, Action<TUnitOfWork> configuration = null, CancellationToken cancellationToken = new ())
    {
        return GetSet().FindAsync(id, cancellationToken);
    }

    public Task<IEnumerable<TResult>> GetMappedAsync<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> map, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new ())
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TResult>> GetMappedAsync<TResult>(ISpecification<TEntity> specification, Expression<Func<TEntity, TResult>> map, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new ())
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TEntity>> GetFilteredAsync(Expression<Func<TEntity, bool>> filter, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new ())
    {
        if (filter == null)
            throw new ArgumentException("Filter expression cannot be null", nameof(filter));

        IMongoQueryable<TEntity> query = GetSet().Where(filter);

        var config = Set<TEntity>.GetConfig(configuration);
        if (config.SortingColumns != null && config.SortingColumns.Any())
        {
            query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(config.SortingColumns.ToArray());
        }

        return await query.ToListAsync(cancellationToken);
    }

    public Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> filter, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new ())
    {
        IMongoQueryable<TEntity> query = GetSet();
        if (filter != null) query = query.Where(filter);

        var config = Set<TEntity>.GetConfig(configuration);
        if (config.SortingColumns != null && config.SortingColumns.Any())
        {
            query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(config.SortingColumns.ToArray());
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<TEntity> GetFirstAsync(ISpecification<TEntity> specification, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new ())
    {
        return GetFirstAsync(specification?.SatisfiedBy(), configuration, cancellationToken);
    }

    public Task<TResult> GetFirstMappedAsync<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> map, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new ())
    {
        throw new NotImplementedException();
    }

    public Task<TResult> GetFirstMappedAsync<TResult>(ISpecification<TEntity> specification, Expression<Func<TEntity, TResult>> map, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(int limit, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new())
    {
        return GetPagedAsync((Expression<Func<TEntity, bool>>)null, 1, limit, configuration, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, int limit, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new())
    {
        return GetPagedAsync(specification.SatisfiedBy(), 1, limit, configuration, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> filter, int limit, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new())
    {
        return GetPagedAsync(filter, 1, limit, configuration, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(int pageIndex, int pageSize, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new())
    {
        return GetPagedAsync((Expression<Func<TEntity, bool>>)null, pageIndex, pageSize, configuration, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, int pageIndex, int pageSize, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new())
    {
        return GetPagedAsync(specification.SatisfiedBy(), pageIndex, pageSize, configuration, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> filter, int pageIndex, int pageSize, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new())
    {
        IMongoQueryable<TEntity> query = GetSet();
        if (filter != null) query = query.Where(filter);

        var config = Set<TEntity>.GetConfig(configuration);
        if (config.SortingColumns != null && config.SortingColumns.Any())
        {
            query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(config.SortingColumns.ToArray());
        }
        if (pageSize > 0)
        {
            int skip = (pageIndex - 1) * pageSize;
            return await query.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new())
    {
        if (filter == null)
            throw new ArgumentException("Filter expression cannot be null", nameof(filter));

        IMongoQueryable<TEntity> query = GetSet().Where(filter);

        var config = Set<TEntity>.GetConfig(configuration);
        if (config.SortingColumns != null && config.SortingColumns.Any())
        {
            query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(config.SortingColumns.ToArray());
        }
        return query.SingleOrDefaultAsync(cancellationToken);
    }

    public Task<TEntity> GetSingleAsync(ISpecification<TEntity> specification, Action<TUnitOfWork> configuration = null,
        CancellationToken cancellationToken = new())
    {
        if (specification == null)
            throw new ArgumentException("The specification cannot be null", nameof(specification));
        return GetSingleAsync(specification.SatisfiedBy(), configuration, cancellationToken);
    }
}