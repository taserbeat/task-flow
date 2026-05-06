using Application.Repositories;
using Domain.Entities.Tenants;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// テナントのリポジトリ
    /// </summary>
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

        public async Task<TenantEm?> GetByIdAsync(TenantId tenantId)
        {
            return await _dbContext.Tenants.FirstOrDefaultAsync(x => x.Id == tenantId);
        }
    }
}