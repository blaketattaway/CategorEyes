using OneCore.CategorEyes.Business.Persistence.Repositories;
using OneCore.CategorEyes.Commons.Entities.Common;

namespace OneCore.CategorEyes.Business.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IHistoricalRepository HistoricalRepository { get; }

        /// <summary>
        /// Retrieves a repository for the specified entity type. If the repository does not exist, a new one is created.
        /// </summary>
        /// <typeparam name="T">The entity type for which the repository is requested, must derive from <see cref="BaseEntity"/>.</typeparam>
        /// <returns>A repository of type <see cref="IAsyncRepository{T}"/> for the specified entity.</returns>
        IAsyncRepository<T> Repository<T>() where T : BaseEntity;

        /// <summary>
        /// Asynchronously completes all pending database operations, committing the transaction.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, returning the number of entities affected. Returns -1 if an exception occurs.</returns>
        Task<int> CompleteAsync();
    }
}
