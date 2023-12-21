using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Options;
using MongoDB.Driver;

namespace eQuantic.Core.Data.MongoDb.Repository;

public class UnitOfWork : IQueryableUnitOfWork
{
    private readonly IMongoDatabase _database;
    private readonly Dictionary<Type,object> _sets = new();
        
    public UnitOfWork(MongoOptions options)
    {
        IMongoClient client = new MongoClient(options.ConnectionString);
        _database = client.GetDatabase(options.Database);
    }

    public int Commit()
    {
        return 0;
    }

    public int Commit(Action<SaveOptions> options)
    {
        return 0;
    }

    public int CommitAndRefreshChanges()
    {
        return 0;
    }

    public int CommitAndRefreshChanges(Action<SaveOptions> options)
    {
        return 0;
    }

    public Task<int> CommitAndRefreshChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(0);
    }

    public Task<int> CommitAndRefreshChangesAsync(Action<SaveOptions> options, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(0);
    }

    public Task<int> CommitAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(0);
    }

    public Task<int> CommitAsync(Action<SaveOptions> options, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(0);
    }

    Data.Repository.ISet<TEntity> IQueryableUnitOfWork.CreateSet<TEntity>()
    {
        var entityType = typeof(TEntity);

        if(_sets.ContainsKey(entityType))
        {
            return (Data.Repository.ISet<TEntity>)_sets[entityType];
        }
        var set = new Set<TEntity>(_database);
        _sets.Add(typeof(TEntity), set);
        return set;
    }

    public IQueryableRepository<TUnitOfWork, TEntity, TKey> GetQueryableRepository<TUnitOfWork, TEntity, TKey>() where TUnitOfWork : IQueryableUnitOfWork where TEntity : class, IEntity, new()
    {
        throw new NotImplementedException();
    }

    public IAsyncQueryableRepository<TUnitOfWork, TEntity, TKey> GetAsyncQueryableRepository<TUnitOfWork, TEntity, TKey>() where TUnitOfWork : IQueryableUnitOfWork where TEntity : class, IEntity, new()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _sets.Clear();
    }

    public void RollbackChanges()
    {
    }

    public IRepository<TUnitOfWork, TEntity, TKey> GetRepository<TUnitOfWork, TEntity, TKey>() where TUnitOfWork : IUnitOfWork where TEntity : class, IEntity, new()
    {
        throw new NotImplementedException();
    }

    public IAsyncRepository<TUnitOfWork, TEntity, TKey> GetAsyncRepository<TUnitOfWork, TEntity, TKey>() where TUnitOfWork : IUnitOfWork where TEntity : class, IEntity, new()
    {
        throw new NotImplementedException();
    }
}