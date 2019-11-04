using System.Threading.Tasks;
using eQuantic.Core.Data.Repository;
using MongoDB.Driver;

namespace eQuantic.Core.Data.MongoDb.Repository
{
    public class UnitOfWork : IQueryableUnitOfWork
    {
        private IMongoClient client;
        private IMongoDatabase database;

        public UnitOfWork(string connectionString, string database)
        {
            this.client = new MongoClient(connectionString);
            this.database = client.GetDatabase(database);
        }

        public int Commit()
        {
            throw new System.NotImplementedException();
        }

        public int CommitAndRefreshChanges()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> CommitAndRefreshChangesAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> CommitAsync()
        {
            throw new System.NotImplementedException();
        }

        ISet<TEntity> IQueryableUnitOfWork.CreateSet<TEntity>()
        {
            return new Set<TEntity>(this.database);
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public TRepository GetRepository<TRepository>() where TRepository : IRepository
        {
            throw new System.NotImplementedException();
        }

        public TRepository GetRepository<TRepository>(string name) where TRepository : IRepository
        {
            throw new System.NotImplementedException();
        }

        public void RollbackChanges()
        {
            throw new System.NotImplementedException();
        }
    }
}