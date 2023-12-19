using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using eQuantic.Core.Data.MongoDb.Repository.Read;
using eQuantic.Core.Data.MongoDb.Repository.Write;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Config;
using eQuantic.Core.Data.Repository.Read;
using eQuantic.Core.Data.Repository.Write;
using eQuantic.Linq.Specification;

namespace eQuantic.Core.Data.MongoDb.Repository;

public class Repository<TUnitOfWork, TEntity, TKey> : IRepository<TUnitOfWork, TEntity, TKey>
    where TUnitOfWork : Configuration<TEntity>, IQueryableUnitOfWork
    where TEntity : class, IEntity, new()
{
    private readonly IReadRepository<TUnitOfWork, TEntity, TKey> _readRepository;
    private readonly IWriteRepository<TUnitOfWork, TEntity> _writeRepository;

    public Repository(TUnitOfWork unitOfWork)
    {
        if (unitOfWork == null)
            throw new ArgumentNullException(nameof(unitOfWork));

        UnitOfWork = unitOfWork;

        _readRepository = new ReadRepository<TUnitOfWork, TEntity, TKey>(UnitOfWork);
        _writeRepository = new WriteRepository<TUnitOfWork, TEntity, TKey>(UnitOfWork);
    }

    /// <summary>
    /// <see cref="eQuantic.Core.Data.Repository.IRepository{TUnitOfWork, TEntity, TKey}"/>
    /// </summary>
    public TUnitOfWork UnitOfWork { get; private set; }

    public void Add(TEntity item)
    {
        _writeRepository.Add(item);
    }

    public IEnumerable<TEntity> AllMatching(ISpecification<TEntity> specification, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.AllMatching(specification, configuration);
    }

    public long Count()
    {
        return _readRepository.Count();
    }

    long IReadRepository<Configuration<TEntity>, TEntity, TKey>.Count(ISpecification<TEntity> specification)
    {
        return Count(specification);
    }

    public long Count(ISpecification<TEntity> specification)
    {
        return _readRepository.Count(specification);
    }

    public long Count(Expression<Func<TEntity, bool>> filter)
    {
        return _readRepository.Count(filter);
    }

    public bool All(ISpecification<TEntity> specification, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.All(specification, configuration);
    }

    public bool All(Expression<Func<TEntity, bool>> filter, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.All(filter, configuration);
    }

    public bool Any(Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.Any(configuration);
    }

    public bool Any(ISpecification<TEntity> specification, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.Any(specification, configuration);
    }

    public bool Any(Expression<Func<TEntity, bool>> filter, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.Any(filter, configuration);
    }

    public TEntity Get(TKey id, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.Get(id, configuration);
    }

    public IEnumerable<TEntity> GetAll(Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetAll(configuration);
    }

    public IEnumerable<TResult> GetMapped<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> map, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetMapped(filter, map, configuration);
    }

    public IEnumerable<TResult> GetMapped<TResult>(ISpecification<TEntity> specification, Expression<Func<TEntity, TResult>> map, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetMapped(specification, map, configuration);
    }

    public IEnumerable<TEntity> GetFiltered(Expression<Func<TEntity, bool>> filter, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetFiltered(filter, configuration);
    }

    public TEntity GetFirst(Expression<Func<TEntity, bool>> filter, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetFirst(filter, configuration);
    }

    public TEntity GetFirst(ISpecification<TEntity> specification, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetFirst(specification, configuration);
    }

    public TResult GetFirstMapped<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> map, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetFirstMapped(filter, map, configuration);
    }

    public TResult GetFirstMapped<TResult>(ISpecification<TEntity> specification, Expression<Func<TEntity, TResult>> map, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetFirstMapped(specification, map, configuration);
    }

    public IEnumerable<TEntity> GetPaged(int limit, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetPaged(limit, configuration);
    }

    public IEnumerable<TEntity> GetPaged(ISpecification<TEntity> specification, int limit, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetPaged(specification, limit, configuration);
    }

    public IEnumerable<TEntity> GetPaged(Expression<Func<TEntity, bool>> filter, int limit, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetPaged(filter, limit, configuration);
    }

    public IEnumerable<TEntity> GetPaged(int pageIndex, int pageSize, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetPaged(pageIndex, pageSize, configuration);
    }

    public IEnumerable<TEntity> GetPaged(ISpecification<TEntity> specification, int pageIndex, int pageSize, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetPaged(specification, pageIndex, pageSize, configuration);
    }

    public IEnumerable<TEntity> GetPaged(Expression<Func<TEntity, bool>> filter, int pageIndex, int pageSize, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetPaged(filter, pageIndex, pageSize, configuration);
    }

    public TEntity GetSingle(Expression<Func<TEntity, bool>> filter, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetSingle(filter, configuration);
    }

    public TEntity GetSingle(ISpecification<TEntity> specification, Action<Configuration<TEntity>> configuration = null)
    {
        return _readRepository.GetSingle(specification, configuration);
    }

    public long DeleteMany(Expression<Func<TEntity, bool>> filter)
    {
        return _writeRepository.DeleteMany(filter);
    }

    long IWriteRepository<TEntity>.DeleteMany(ISpecification<TEntity> specification)
    {
        return DeleteMany(specification);
    }

    public long DeleteMany(ISpecification<TEntity> specification)
    {
        return _writeRepository.DeleteMany(specification);
    }

    public void Dispose()
    {
        UnitOfWork?.Dispose();
    }

    public void Merge(TEntity persisted, TEntity current)
    {
        _writeRepository.Merge(persisted, current);
    }

    public void Modify(TEntity item)
    {
        _writeRepository.Modify(item);
    }

    public void Remove(TEntity item)
    {
        _writeRepository.Remove(item);
    }

    public void TrackItem(TEntity item)
    {
        _writeRepository.TrackItem(item);
    }

    public long UpdateMany(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> updateFactory)
    {
        return _writeRepository.UpdateMany(filter, updateFactory);
    }

    long IWriteRepository<TEntity>.UpdateMany(ISpecification<TEntity> specification, Expression<Func<TEntity, TEntity>> updateFactory)
    {
        return UpdateMany(specification, updateFactory);
    }

    public long UpdateMany(ISpecification<TEntity> specification, Expression<Func<TEntity, TEntity>> updateFactory)
    {
        return _writeRepository.UpdateMany(specification, updateFactory);
    }
}