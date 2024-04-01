using OneCore.CategorEyes.Business.Persistence.Repositories;
using OneCore.CategorEyes.Commons.Entities;
using OneCore.CategorEyes.Infrastructure.Persistence.Context;

namespace OneCore.CategorEyes.Infrastructure.Persistence.Repositories
{
    public class HistoricalRepository : RepositoryBase<Historical>, IHistoricalRepository
    {
        public HistoricalRepository(CategorEyesDbContext context) : base(context)
        {
        }
    }
}
