using OneCore.CategorEyes.Business.Persistence.Repositories;
using OneCore.CategorEyes.Commons.Entities.Common;

namespace OneCore.CategorEyes.Business.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IAsyncRepository<T> Repository<T>() where T : BaseEntity;
        IHistoricalRepository HistoricalRepository { get; }
        Task<int> CompleteAsync();
    }
}
