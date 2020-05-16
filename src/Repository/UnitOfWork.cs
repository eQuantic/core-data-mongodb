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

        ISet<TEntity> IQueryableUnitOfWork.CreateSet<TEntity>()
        {
            return new Set<TEntity>(_database);
        }

        public void Dispose()
        {
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