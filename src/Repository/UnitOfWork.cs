using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Ioc;
using MongoDB.Driver;

namespace eQuantic.Core.Data.MongoDb.Repository
{
    public class UnitOfWork : IQueryableUnitOfWork
    {
        private readonly IContainer _container;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private Dictionary<Type,object> _sets = new Dictionary<Type,object>();
        
        public UnitOfWork(string connectionString, string database, IContainer container)
        {
            _container = container;
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(database);
        }

        public int Commit()
        {
            return 0;
        }

        public int CommitAndRefreshChanges()
        {
            return 0;
        }

        public Task<int> CommitAndRefreshChangesAsync()
        {
            return Task.FromResult(0);
        }

        public Task<int> CommitAsync()
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

        public TRepository GetRepository<TRepository>() where TRepository : IRepository
        {
            return _container.Resolve<TRepository>();
        }

        public TRepository GetRepository<TRepository>(string name) where TRepository : IRepository
        {
            return _container.Resolve<TRepository>(name);
        }

        public void RollbackChanges()
        {
        }
    }
}