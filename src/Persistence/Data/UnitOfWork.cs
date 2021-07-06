using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities.Base;
using Domain.Interfaces;
using Persistence.Identity;

namespace Persistence.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppIdentityDbContext _context;
        private Dictionary<string, object> _repositories;
        public UnitOfWork(AppIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if (_repositories == null) _repositories = new Dictionary<string, object>();

            var typeName = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(typeName)){
                var repositoryType = typeof(GenericRepository<>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);

                _repositories.Add(typeName, repositoryInstance);
            }

            return (GenericRepository<TEntity>)_repositories[typeName];
        }
    }
}