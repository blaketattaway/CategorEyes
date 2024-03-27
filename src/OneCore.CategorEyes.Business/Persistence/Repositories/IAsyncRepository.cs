using OneCore.CategorEyes.Commons.Entities.Common;
using OneCore.CategorEyes.Commons.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Business.Persistence.Repositories
{
    public interface IAsyncRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();

        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);

        Task<(IReadOnlyList<T>, int)> GetPagedAsync(int skip, int top, Expression<Func<T, bool>>? predicate = null,
            SortDescriptor? sortDescriptor = null,
            bool disableTracking = true);

        Task<T?> GetByIdAsync(long id);

        Task AddAsync(T entity);

        void UpdateEntity(T entity);

        void DeleteEntity(T entity);
    }
}
