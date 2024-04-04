using Microsoft.EntityFrameworkCore;
using OneCore.CategorEyes.Business.Persistence.Repositories;
using OneCore.CategorEyes.Commons.Consts;
using OneCore.CategorEyes.Commons.Entities.Common;
using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Infrastructure.Extensions;
using OneCore.CategorEyes.Infrastructure.Persistence.Context;
using System.Linq.Expressions;

namespace OneCore.CategorEyes.Infrastructure.Persistence.Repositories
{
    public class RepositoryBase<T> : IAsyncRepository<T> where T : BaseEntity
    {
        protected readonly CategorEyesDbContext _dbContext;

        public RepositoryBase(CategorEyesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
            SortDescriptor? sortDescriptor = null)
        {
            IQueryable<T> query = ApplyBaseQueryOptions(predicate, disableTracking: true, sortDescriptor);
            return await query.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        /// <summary>
        /// Applies base query options such as filtering, tracking, and sorting to a query.
        /// </summary>
        /// <param name="predicate">An optional expression to filter entities.</param>
        /// <param name="disableTracking">A flag indicating whether to disable change tracking.</param>
        /// <param name="sortDescriptor">An optional descriptor to apply sorting.</param>
        /// <returns>An <see cref="IQueryable{T}"/> with the applied query options.</returns>
        private IQueryable<T> ApplyBaseQueryOptions(Expression<Func<T, bool>>? predicate,
                                                    bool disableTracking,
                                                    SortDescriptor? sortDescriptor = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (sortDescriptor != null)
            {
                query = ApplySorting(query, sortDescriptor);
            }

            return query;
        }

        /// <summary>
        /// Applies sorting to a query based on the provided sort descriptor.
        /// </summary>
        /// <param name="query">The query to apply sorting to.</param>
        /// <param name="sortDescriptor">The descriptor containing sorting details.</param>
        /// <returns>An <see cref="IQueryable{T}"/> with the applied sorting.</returns>
        private static IQueryable<T> ApplySorting(IQueryable<T> query, SortDescriptor sortDescriptor)
        {
            return sortDescriptor.SortOrder == SortOrder.Ascending
                    ? query.OrderBy(sortDescriptor.Property)
                    : query.OrderByDescending(sortDescriptor.Property);
        }

        /// <inheritdoc/>
        public async Task<(IReadOnlyList<T>, int)> GetPagedAsync(int skip, int take,
            Expression<Func<T, bool>>? predicate = null,
            SortDescriptor? sortDescriptor = null,
            bool disableTracking = true)
        {
            IQueryable<T> query = ApplyBaseQueryOptions(predicate, disableTracking, sortDescriptor);

            var pagedItems = await query.Skip(skip).Take(take).ToListAsync();
            var totalCount = await query.CountAsync();

            return (pagedItems, totalCount);
        }
    }
}
