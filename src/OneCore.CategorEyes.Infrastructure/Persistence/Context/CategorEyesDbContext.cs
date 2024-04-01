using Microsoft.EntityFrameworkCore;
using OneCore.CategorEyes.Commons.Entities;
using OneCore.CategorEyes.Commons.Entities.Common;

namespace OneCore.CategorEyes.Infrastructure.Persistence.Context
{
    public class CategorEyesDbContext : DbContext
    {
        public CategorEyesDbContext(DbContextOptions<CategorEyesDbContext> options) : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreationDate = DateTime.Now;
                        break;
                    default:
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<Historical>? Historical { get; set; }
    }
}
