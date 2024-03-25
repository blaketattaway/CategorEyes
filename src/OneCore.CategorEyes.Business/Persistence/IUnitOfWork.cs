using OneCore.CategorEyes.Business.Persistence.Repositories;
using OneCore.CategorEyes.Commons.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Business.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IAsyncRepository<T> Repository<T>() where T : BaseEntity;
        IHistoricalRepository HistoricalRepository { get; }
        Task<int> CompleteAsync();
    }
}
