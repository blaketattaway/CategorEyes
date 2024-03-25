using OneCore.CategorEyes.Business.Persistence.Repositories;
using OneCore.CategorEyes.Commons.Entities;
using OneCore.CategorEyes.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Infrastructure.Persistence.Repositories
{
    public class HistoricalRepository : RepositoryBase<Historical>, IHistoricalRepository
    {
        public HistoricalRepository(CategorEyesDbContext context) : base(context)
        {
        }

        public Task<List<Historical>> GetByFilter()
        {
            throw new NotImplementedException();
        }
    }
}
