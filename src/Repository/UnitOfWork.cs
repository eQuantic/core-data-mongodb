using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Data.Repository.Options;
using eQuantic.Core.Ioc;
using MongoDB.Driver;

namespace eQuantic.Core.Data.MongoDb.Repository;

public class UnitOfWork : IQueryableUnitOfWork
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly Dictionary<Type,object> _sets = new();
        
    public UnitOfWork(string connectionString, string database)
    {
        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(database);
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

    public void Dispose()
    {
        _sets.Clear();
    }

    public void RollbackChanges()
    {
    }
}