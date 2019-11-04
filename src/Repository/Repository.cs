using System;
using System.Linq.Expressions;
using eQuantic.Core.Data.MongoDb.Repository.Read;
using eQuantic.Core.Data.Repository;
using eQuantic.Core.Linq.Specification;

namespace eQuantic.Core.Data.MongoDb.Repository
{
    public class Repository<TUnitOfWork, TEntity, TKey> : ReadRepository<TUnitOfWork, TEntity, TKey>, IRepository<TUnitOfWork, TEntity, TKey>
        where TUnitOfWork : IQueryableUnitOfWork
        where TEntity : class, IEntity, new()
    {
        public Repository(TUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Add(TEntity item)
        {
            throw new NotImplementedException();
        }

        public int DeleteMany(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public int DeleteMany(ISpecification<TEntity> specification)
        {
            throw new NotImplementedException();
        }

        public void Merge(TEntity persisted, TEntity current)
        {
            throw new NotImplementedException();
        }

        public void Modify(TEntity item)
        {
            throw new NotImplementedException();
        }

        public void Remove(TEntity item)
        {
            throw new NotImplementedException();
        }

        public void TrackItem(TEntity item)
        {
            throw new NotImplementedException();
        }

        public int UpdateMany(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> updateFactory)
        {
            throw new NotImplementedException();
        }

        public int UpdateMany(ISpecification<TEntity> specification, Expression<Func<TEntity, TEntity>> updateFactory)
        {
            throw new NotImplementedException();
        }
    }
}