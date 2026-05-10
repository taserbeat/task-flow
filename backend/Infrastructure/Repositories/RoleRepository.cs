using Application.Repositories;
using Domain.Entities.Roles;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// ロールのリポジトリ
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _dbContext;

        public RoleRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(RoleEm roleEm)
        {
            await _dbContext.Roles.AddAsync(roleEm);
        }

        public async Task<RoleEm?> GetByIdAsync(RoleId roleId)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == roleId);
        }

        public async Task<RoleEm?> GetByNameAsync(RoleNameEnum roleName)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(x => x.Name == roleName);
        }
    }
}