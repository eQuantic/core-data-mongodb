using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Config;
using eQuantic.Core.Data.Repository.Read;
using eQuantic.Linq.Extensions;
using eQuantic.Linq.Specification;
using MongoDB.Driver.Linq;

namespace eQuantic.Core.Data.MongoDb.Repository.Read;

public class QueryableReadRepository<TUnitOfWork, TEntity, TKey> : 
    IQueryableReadRepository<TUnitOfWork, TEntity, TKey>
    where TUnitOfWork : IQueryableUnitOfWork
    where TEntity : class, IEntity, new()
{
    private Set<TEntity> _dbSet = null;

    /// <summary>
    /// Create a new instance of repository
    /// </summary>
    /// <param name="unitOfWork">Associated Unit Of Work</param>
    public QueryableReadRepository(TUnitOfWork unitOfWork)
    {
        if (unitOfWork == null)
            throw new ArgumentNullException(nameof(unitOfWork));

        UnitOfWork = unitOfWork;
    }

    /// <summary>
    /// <see cref="eQuantic.Core.Data.Repository.Read.IReadRepository{TUnitOfWork, TEntity, TKey}"/>
    /// </summary>
    public TUnitOfWork UnitOfWork { get; private set; }

    public IEnumerable<TEntity> AllMatching(ISpecification<TEntity> specification, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        IMongoQueryable<TEntity> query = GetSet().Where(specification.SatisfiedBy());
        var config = Set<TEntity>.GetConfig(configuration);
        if (config.SortingColumns != null && config.SortingColumns.Any())
        {
            query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(config.SortingColumns.ToArray());
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

    public bool All(ISpecification<TEntity> specification, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetSet().All(specification.SatisfiedBy());
    }

    public bool All(Expression<Func<TEntity, bool>> filter, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetSet().All(filter);
    }

    public bool Any(Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetSet().Any();
    }

    public bool Any(ISpecification<TEntity> specification, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetSet().Any(specification.SatisfiedBy());
    }

    public bool Any(Expression<Func<TEntity, bool>> filter, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetSet().Any(filter);
    }

    public TEntity Get(TKey id, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return id != null ? GetSet().Find(id) : null;
    }

    public IEnumerable<TEntity> GetAll(Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        IMongoQueryable<TEntity> query = GetSet();
        var config = Set<TEntity>.GetConfig(configuration);
        if (config.SortingColumns != null && config.SortingColumns.Any())
        {
            query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(config.SortingColumns.ToArray());
        }
        return query.ToList();
    }

    public IEnumerable<TResult> GetMapped<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> map, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<TResult> GetMapped<TResult>(ISpecification<TEntity> specification, Expression<Func<TEntity, TResult>> map, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<TEntity> GetFiltered(Expression<Func<TEntity, bool>> filter, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        if (filter == null)
            throw new ArgumentException("Filter expression cannot be null", nameof(filter));

        IMongoQueryable<TEntity> query = GetSet().Where(filter);
        var config = Set<TEntity>.GetConfig(configuration);
        if (config.SortingColumns != null && config.SortingColumns.Any())
        {
            query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(config.SortingColumns.ToArray());
        }
        return query;
    }

    public TEntity GetFirst(Expression<Func<TEntity, bool>> filter, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        IMongoQueryable<TEntity> query = filter == null ? GetSet() : GetSet().Where(filter);
        var config = Set<TEntity>.GetConfig(configuration);
        if (config.SortingColumns != null && config.SortingColumns.Any())
        {
            query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(config.SortingColumns.ToArray());
        }

        return query.FirstOrDefault();
    }

    public TEntity GetFirst(ISpecification<TEntity> specification, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetFirst(specification?.SatisfiedBy(), configuration);
    }

    public TResult GetFirstMapped<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> map, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        throw new NotImplementedException();
    }

    public TResult GetFirstMapped<TResult>(ISpecification<TEntity> specification, Expression<Func<TEntity, TResult>> map, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<TEntity> GetPaged(int limit, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetPaged((Expression<Func<TEntity, bool>>)null, 1, limit, configuration);
    }

    public IEnumerable<TEntity> GetPaged(ISpecification<TEntity> specification, int limit, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetPaged(specification.SatisfiedBy(), 1, limit, configuration);
    }

    public IEnumerable<TEntity> GetPaged(Expression<Func<TEntity, bool>> filter, int limit, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetPaged(filter, 1, limit, configuration);
    }

    public IEnumerable<TEntity> GetPaged(int pageIndex, int pageSize, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetPaged((Expression<Func<TEntity, bool>>)null, pageIndex, pageSize, configuration);
    }

    public IEnumerable<TEntity> GetPaged(ISpecification<TEntity> specification, int pageIndex, int pageSize, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        return GetPaged(specification.SatisfiedBy(), pageIndex, pageSize, configuration);
    }

    public IEnumerable<TEntity> GetPaged(Expression<Func<TEntity, bool>> filter, int pageIndex, int pageSize, Action<QueryableConfiguration<TEntity>> configuration = null)
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
            return query.Skip(skip).Take(pageSize);
        }

        return query;
    }

    public TEntity GetSingle(Expression<Func<TEntity, bool>> filter, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        if (filter == null)
            throw new ArgumentException("Filter expression cannot be null", nameof(filter));

        IMongoQueryable<TEntity> query = GetSet().Where(filter);
        var config = Set<TEntity>.GetConfig(configuration);
        if (config.SortingColumns != null && config.SortingColumns.Any())
        {
            query = (IOrderedMongoQueryable<TEntity>)query.OrderBy(config.SortingColumns.ToArray());
        }

        return query.SingleOrDefault();
    }

    public TEntity GetSingle(ISpecification<TEntity> specification, Action<QueryableConfiguration<TEntity>> configuration = null)
    {
        if (specification == null)
            throw new ArgumentException("The specification cannot be null", nameof(specification));

        return GetSingle(specification.SatisfiedBy(), configuration);
    }

    /// <summary>
    /// <see cref="M:System.IDisposable.Dispose"/>
    /// </summary>
    public void Dispose()
    {
        UnitOfWork?.Dispose();
    }

    protected Set<TEntity> GetSet()
    {
        return _dbSet ??= (Set<TEntity>)UnitOfWork.CreateSet<TEntity>();
    }
}