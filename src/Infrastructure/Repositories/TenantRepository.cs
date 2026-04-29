using Application.Repositories;
using Domain.Entities;
using Infrastructure.DbContexts;

namespace Infrastructure.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly AppDbContext _dbContext;

        public TenantRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TenantEm tenantEm)
        {
            await _dbContext.Tenants.AddAsync(tenantEm);
        }
    }
}