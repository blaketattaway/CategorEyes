using Microsoft.EntityFrameworkCore;
using OneCore.CategorEyes.Business.Persistence.Repositories;
using OneCore.CategorEyes.Commons.Entities.Common;
using OneCore.CategorEyes.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OneCore.CategorEyes.Infrastructure.Extensions;
using OneCore.CategorEyes.Commons.Requests;
using OneCore.CategorEyes.Commons.Consts;

namespace OneCore.CategorEyes.Infrastructure.Persistence.Repositories
{
    public class RepositoryBase<T> : IAsyncRepository<T> where T : BaseEntity
    {
        protected readonly CategorEyesDbContext _dbContext;

        public RepositoryBase(CategorEyesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public async Task<(IReadOnlyList<T>, int)> GetPagedAsync(int skip, int take, 
            Expression<Func<T, bool>>? predicate = null,
            SortDescriptor? sortDescriptor = null,
            bool disableTracking = true)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (disableTracking) query = query.AsNoTracking();

            if (predicate is not null) query = query.Where(predicate);

            if (sortDescriptor is not null)
            {
                switch (sortDescriptor.SortOrder)
                {
                    case SortOrder.Ascending:
                        return (await query.OrderBy(sortDescriptor.Property)
                                    .Skip(skip)
                                    .Take(take)
                                    .ToListAsync(), 
                                await query.CountAsync());
                    case SortOrder.Descending:
                        return (await query.OrderByDescending(sortDescriptor.Property)
                                    .Skip(skip)
                                    .Take(take)
                                    .ToListAsync(), 
                                await query.CountAsync());
                }
            }

            return (await query.Skip(skip).Take(take).ToListAsync(), await query.CountAsync());
        }
    }
}
