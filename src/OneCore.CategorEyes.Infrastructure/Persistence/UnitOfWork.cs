using OneCore.CategorEyes.Business.Persistence;
using OneCore.CategorEyes.Business.Persistence.Repositories;
using OneCore.CategorEyes.Commons.Entities.Common;
using OneCore.CategorEyes.Infrastructure.Persistence.Context;
using OneCore.CategorEyes.Infrastructure.Persistence.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private Hashtable _repositories;
        private readonly CategorEyesDbContext _context;

        public UnitOfWork(CategorEyesDbContext context)
        {
            _context = context;
            HistoricalRepository = new HistoricalRepository(context);
        }

        public IHistoricalRepository HistoricalRepository { get; }

        public async Task<int> CompleteAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                _context.Dispose();
            }

        }

        public IAsyncRepository<T> Repository<T>() where T : BaseEntity
        {
            if (_repositories is null)
            {
                _repositories = new();
            }

            var type = typeof(T).Name;
            if (!_repositories.Contains(type))
            {
                var repositoryType = typeof(RepositoryBase<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
                _repositories.Add(type, repositoryInstance);
            }

            return (IAsyncRepository<T>)_repositories[type];
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
