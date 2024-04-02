using OneCore.CategorEyes.Business.Persistence;
using OneCore.CategorEyes.Business.Persistence.Repositories;
using OneCore.CategorEyes.Commons.Entities.Common;
using OneCore.CategorEyes.Infrastructure.Persistence.Context;
using OneCore.CategorEyes.Infrastructure.Persistence.Repositories;
using System.Collections;

namespace OneCore.CategorEyes.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private Hashtable _repositories;
        private readonly CategorEyesDbContext _context;

        /// <summary>
        /// Initializes a new instance of the UnitOfWork class with a specific database context.
        /// </summary>
        /// <param name="context">The database context of type <see cref="CategorEyesDbContext"/> used to interact with the database.</param>
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources used by the UnitOfWork and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }

        /// <summary>
        /// The destructor for the UnitOfWork class that calls Dispose, ensuring all resources are freed.
        /// </summary>
        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
